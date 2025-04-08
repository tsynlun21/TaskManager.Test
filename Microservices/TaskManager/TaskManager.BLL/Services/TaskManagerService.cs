using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Shared.Models.TaskManager;
using TaskManager.DAL.Converters;
using TaskManager.DAL.DataContext;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.Models;

namespace TaskManager.BLL.Services;

public class TaskManagerService(TaskManagerContext context, bool useRawSql = true) : ITaskManagerService
{
    public async Task<Result<TaskModel>> AddTaskAsync(TaskModel taskModel)
    {
        var entity = taskModel.ToTaskEntity();

        var inserted = await context.TaskEntities.AddAsync(entity);
        await context.SaveChangesAsync();

        var result = inserted.Entity.ToTaskModel();
        return result;
    }

    public async Task<Result<TaskCompletionStatus>> ChangeTaskStatusAsync(Guid id, TaskCompletionStatus completionStatus)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var entity = useRawSql ? await context.TaskEntities
                                  .FromSqlRaw("SELECT * FROM \"Tasks\" WHERE \"Id\" = {0} FOR UPDATE", id)
                                  .FirstOrDefaultAsync() : await context.TaskEntities.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return Result.NotFound($"Task with id: {id} not found");

        var taskModel = entity.ToTaskModel();
        var changeRes = taskModel.ChangeStatus(completionStatus);
        if (changeRes.IsError())
            return changeRes;

        var previousStatus = entity.CompletionStatus;
        
        entity.CompletionStatus = completionStatus;
        entity.UpdatedAt        = DateTime.UtcNow;

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Result.Success(previousStatus);
    }

    public async Task<Result<List<TaskModel>>> GetTasksAsync()
    {
        var tasks = await context.TaskEntities.AsNoTracking().Where(x => x.DeletedAt == null)
                                 .Select(x => x.ToTaskModel()).ToListAsync();

        if (!tasks.Any())
            return Result.NoContent();

        return Result.Success(tasks);
    }

    public async Task<Result> DeleteTaskAsync(Guid id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var entity = useRawSql ? await context.TaskEntities.FromSqlRaw("SELECT * FROM \"Tasks\" WHERE \"Id\" = {0} FOR UPDATE", id)
                                  .FirstOrDefaultAsync() 
                                  : await context.TaskEntities.FirstOrDefaultAsync(x => x.Id == id);
        
        if (entity == null)
            return Result.NotFound($"Task with id: {id} not found");
        
        var taskModel = entity.ToTaskModel();
        taskModel.MarkAsDeleted();
        entity.DeletedAt = taskModel.DeletedAt;
        
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return Result.Success();
    }

    public async Task<Result<(string previousTitle, string previosDescription)>> UpdateTaskAsync(Guid id, string title, string description)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        var entity = useRawSql ? await context.TaskEntities.FromSqlRaw("SELECT * FROM \"Tasks\" WHERE \"Id\" = {0} FOR UPDATE", id)
                                              .FirstOrDefaultAsync() 
            : await context.TaskEntities.FirstOrDefaultAsync(x => x.Id == id);
        
        if (entity == null)
            return Result.NotFound($"Task with id: {id} not found");
        
        var taskModel = entity.ToTaskModel();
        
        var updateRes = taskModel.UpdateInfo(title, description);

        if (updateRes.IsInvalid())
            return updateRes;
        
        var previousTitle = entity.Title;
        var previousDescription = entity.Description;

        entity.Title = title;
        entity.Description = description;
        
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return Result.Success((previousTitle, previousDescription));
    }
}