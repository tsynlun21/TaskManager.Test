using System.ComponentModel.DataAnnotations;
using Shared.Models.TaskManager;

namespace Shared.Masstransit.TaskLogger.Requests;

public class TaskLogRequest
{
    [Required]
    public Guid TaskId { get; init; }
    
    [Required]
    public TaskAction TaskAction { get; init; }
    
    [Required]
    public string SerializedTaskDescription { get; init; }
}