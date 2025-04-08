using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using Shared.Models.TaskManager;

namespace TaskLogger.Domain.Models;

public class TaskLogModel
{
    private TaskLogModel(Guid id, TaskAction taskAction, string serializedTaskDescription)
    {
        Id                             = id;
        TaskAction                     = taskAction;
        SerializedTaskDescription = serializedTaskDescription;
    }
    
    public Guid Id { get; }
    public TaskAction TaskAction { get; }
    public string SerializedTaskDescription { get; }

    public static Result<TaskLogModel> Create(Guid id, TaskAction taskAction, string serializedTaskDescription)
    {
        var validationErrors = new List<ValidationError>();
        
        if (id == Guid.Empty)
            validationErrors.Add(new ValidationError("Guid cannot be empty"));
        
        if (taskAction == TaskAction.Unknown)
            validationErrors.Add(new ValidationError("TaskAction is unknown"));
        
        if (string.IsNullOrWhiteSpace(serializedTaskDescription))
            validationErrors.Add(new ValidationError("SerializedTaskDescription cannot be empty"));

        if (validationErrors.Any())
            return Result.Invalid(validationErrors.ToArray());
        
        var model = new TaskLogModel(id, taskAction, serializedTaskDescription);
        return Result.Success(model);
    }
    
}