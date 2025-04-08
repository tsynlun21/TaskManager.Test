using System.Text.Json;
using Infrustructure.Masstransit;
using MassTransit;
using Shared.Masstransit.TaskLogger.Requests;
using Shared.Masstransit.TaskManager.Requests;
using Shared.Models.TaskManager;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Masstransit.Consumers;

public class ChangeStatusConsumer(ITaskManagerService service, IBus bus) : IConsumer<ChangeStatusMasstransitRequest>
{
    private readonly Uri rabbitLogTaskUri = new Uri($"queue:{RabbitQueueNames.TASK_LOGGER_QUEUE}");

    public async Task Consume(ConsumeContext<ChangeStatusMasstransitRequest> context)
    {
        var changeStatusResult = await service.ChangeTaskStatusAsync(context.Message.Id, context.Message.Status);

        if (changeStatusResult.IsSuccess)
            await LogHelper.SendLogRequest(bus, new TaskLogRequest()
            {
                TaskId     = context.Message.Id,
                TaskAction = TaskAction.Changed,
                SerializedTaskDescription = $"Changed status from {changeStatusResult.Status} to {context.Message.Status}"
            });

        await context.RespondAsync(changeStatusResult);
    }
}