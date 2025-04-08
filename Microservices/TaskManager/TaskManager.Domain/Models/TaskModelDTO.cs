using Shared.Models.TaskManager;

namespace TaskManager.Domain.Models;

public record TaskModelDTO
(
    Guid Id,
    string Title,
    string Description,
    TaskCompletionStatus CompletionStatus,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt
);