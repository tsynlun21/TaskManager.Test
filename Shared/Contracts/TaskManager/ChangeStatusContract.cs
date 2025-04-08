using System.ComponentModel.DataAnnotations;
using Shared.Models.TaskManager;

namespace Shared.Contracts.TaskManager;

/// <summary>
/// Contract for changing task`s status request
/// </summary>
public record ChangeStatusContract()
{
    /// <summary>Guid of the task</summary>
    [Required]
    public Guid TaskId { get; init; }

    /// <summary>New status</summary>
    [Required]
    public TaskCompletionStatus Status { get; init; }
}