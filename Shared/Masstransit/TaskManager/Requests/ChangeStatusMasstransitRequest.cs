using Shared.Models.TaskManager;

namespace Shared.Masstransit.TaskManager.Requests;

public record ChangeStatusMasstransitRequest(Guid Id, TaskCompletionStatus Status); 