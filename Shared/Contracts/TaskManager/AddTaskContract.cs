using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.TaskManager;

/// <summary>
/// Contract for adding new task request
/// </summary>
/// <param name="Title">Title of the task</param>
/// <param name="Description">Description of the task</param>
public record AddTaskContract([Required] string Title, [Required] string Description);