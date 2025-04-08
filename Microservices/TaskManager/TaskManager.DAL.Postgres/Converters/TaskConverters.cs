using Ardalis.Result;
using TaskManager.DAL.Entities;
using TaskManager.Domain.Models;

namespace TaskManager.DAL.Converters;

public static class TaskConverters
{
    public static TaskModel ToTaskModel(this TaskEntity taskEntity)
    {
        var result = TaskModel.Create(taskEntity.Id, taskEntity.Title, taskEntity.Description,
            taskEntity.CompletionStatus,
            taskEntity.CreatedAt, taskEntity.UpdatedAt, taskEntity.DeletedAt);

        if (result.IsInvalid())
        {
            throw new Exception(string.Join(',', result.ValidationErrors));
        }

        return result.Value;
    }

    public static TaskEntity ToTaskEntity(this TaskModel model)
    {
        return new TaskEntity()
        {
            Id               = model.Id,
            Title            = model.Title,
            Description      = model.Description,
            CompletionStatus = model.CompletionStatus,
            CreatedAt        = model.CreatedAt,
            UpdatedAt        = model.UpdatedAt,
            DeletedAt        = model.DeletedAt
        };
    }

    public static TaskModelDTO ToTaskModelDTO(this TaskModel taskModel)
    {
        return new TaskModelDTO(taskModel.Id, taskModel.Title, taskModel.Description, taskModel.CompletionStatus,
            taskModel.CreatedAt, taskModel.UpdatedAt, taskModel.DeletedAt);
    }
}