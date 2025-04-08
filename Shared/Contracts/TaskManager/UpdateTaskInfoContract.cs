using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.TaskManager;
/// <summary>
/// Contract to change info of the task
/// </summary>
/// <param name="TaskId">Guid of the task</param>
/// <param name="Title">New title</param>
/// <param name="Description">New description</param>
public record UpdateTaskInfoContract([Required] Guid TaskId, [Required] string Title, [Required] string Description);