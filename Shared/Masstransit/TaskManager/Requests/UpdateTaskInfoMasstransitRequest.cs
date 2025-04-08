namespace Shared.Masstransit.TaskManager.Requests;

public record UpdateTaskInfoMasstransitRequest(Guid Id, string Title, string Description);