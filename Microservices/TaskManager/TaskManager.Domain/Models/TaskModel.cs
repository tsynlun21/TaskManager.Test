using Ardalis.Result;
using Persistance;
using Shared.Models.TaskManager;

namespace TaskManager.Domain.Models;

public class TaskModel
{
    private TaskModel(Guid id, string title, string description, TaskCompletionStatus completionStatus, DateTime createdAt, DateTime? updatedAt, DateTime? deletedAt)
    {

        Id          = id;
        Title       = title;
        Description = description;
        CompletionStatus      = completionStatus;
        CreatedAt   = createdAt;
        UpdatedAt   = updatedAt;
        DeletedAt   = deletedAt;
    }
    
    public Guid       Id          { get; }
    public string     Title       { get; private set; }
    public string     Description { get; private set; }
    public TaskCompletionStatus CompletionStatus      { get; private set; }
    public DateTime   CreatedAt   { get; }
    public DateTime?  UpdatedAt   { get; private set; }
    public DateTime?  DeletedAt   { get; private set; }

    public static Result<TaskModel> Create(Guid id, string title, string description, TaskCompletionStatus completionStatus, DateTime createdAt, DateTime? updatedAt, DateTime? deletedAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Invalid([new ValidationError("Title is required.")]);
        };
        if (string.IsNullOrWhiteSpace(description))
            return Result.Invalid([new ValidationError("Description is required.")]);

        if (title.Length > Limitations.MAX_TITLE_LENGTH) 
            return Result.Invalid([new ValidationError("Title is too long.")]);
        
        if (description.Length > Limitations.MAX_DESCRIPTION_LENGTH) 
            return Result.Invalid([new ValidationError("Description is too long.")]);
        
        var taskModel = new TaskModel(id, title, description, completionStatus, createdAt, updatedAt, deletedAt);
        
        return Result.Success(taskModel);
    }

    public TaskModel()
    {
        
    }
    
    public Result ChangeStatus(TaskCompletionStatus newCompletionStatus)
    {
        // будем считать, что статус должен меняться постепенно Unhandled -> InProgress -> Completed
        
        if (CompletionStatus == TaskCompletionStatus.Unhandled && newCompletionStatus != TaskCompletionStatus.InProgress)
            return Result.Error("Status after Unhandled can be only InProgress.");
        
        if (CompletionStatus == TaskCompletionStatus.InProgress && newCompletionStatus != TaskCompletionStatus.Completed)
            return Result.Error("Status after InProgress can be only Completed.");
        
        if (CompletionStatus == TaskCompletionStatus.Completed)
            return Result.Error("Task is already completed and status cannot be changed.");

        CompletionStatus = newCompletionStatus;
        return Result.Success();
    }
    
    /// <summary>
    /// Marks model as deleted
    /// </summary>
    public void MarkAsDeleted()
    {
        DeletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates title and description of model
    /// </summary>
    /// <param name="title">New title</param>
    /// <param name="description">New description</param>
    public Result UpdateInfo(string title, string description)
    {
        var validationErrors = new List<ValidationError>();
        
        if (string.IsNullOrWhiteSpace(title))
            validationErrors.Add(new ValidationError("Title is required."));
        
        if (string.IsNullOrWhiteSpace(description))
            validationErrors.Add(new ValidationError("Description is required."));
        
        if (title.Length > Limitations.MAX_TITLE_LENGTH)
            validationErrors.Add(new ValidationError("Title is too long."));
        
        if (description.Length > Limitations.MAX_DESCRIPTION_LENGTH)
            validationErrors.Add(new ValidationError("Description is too long."));
        
        if (validationErrors.Any())
            return Result.Invalid(validationErrors);
            
        
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}