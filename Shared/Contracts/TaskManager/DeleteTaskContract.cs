using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.TaskManager;

/// <summary>
/// Contract to delete task request 
/// </summary>
/// <param name="TaskId">Guid of the task</param>
public record DeleteTaskContract()
{
    /// <summary>Guid of the task</summary>
    [Required]
    public Guid TaskId { get; init; }
}