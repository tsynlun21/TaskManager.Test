using Ardalis.Result;
using MassTransit;
using Shared.Masstransit.TaskLogger.Requests;
using Shared.Masstransit.TaskManager.Requests;
using Shared.Models.TaskManager;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Masstransit.Consumers;

public class UpdateTaskInfoConsumer(ITaskManagerService service, IBus bus) : IConsumer<UpdateTaskInfoMasstransitRequest>
{
    public async Task Consume(ConsumeContext<UpdateTaskInfoMasstransitRequest> context)
    {
        var updatedTaskResult =
            await service.UpdateTaskAsync(context.Message.Id, context.Message.Title, context.Message.Description);

        if (updatedTaskResult.IsInvalid())
            await context.RespondAsync(Result.Invalid(updatedTaskResult.ValidationErrors));
        
        if (updatedTaskResult.IsNotFound())
            await context.RespondAsync(Result.NotFound());

        await LogHelper.SendLogRequest(bus, new TaskLogRequest()
        {
            TaskId     = context.Message.Id,
            TaskAction = TaskAction.Changed,
            SerializedTaskDescription =
                $"Info changed, Title : from {updatedTaskResult.Value.previousTitle} to {context.Message.Title}, Description : from {updatedTaskResult.Value.previosDescription} to {context.Message.Description}"
        });
        
        await context.RespondAsync(Result.Success());
    }
}