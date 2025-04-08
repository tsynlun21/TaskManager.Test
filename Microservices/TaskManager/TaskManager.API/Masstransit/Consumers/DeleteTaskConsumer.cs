using System.Text.Json;
using MassTransit;
using Shared.Masstransit.TaskLogger.Requests;
using Shared.Masstransit.TaskManager.Requests;
using Shared.Models.TaskManager;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Masstransit.Consumers;

public class DeleteTaskConsumer(ITaskManagerService service, IBus bus) : IConsumer<DeleteTaskMasstransitRequest>
{
    public async Task Consume(ConsumeContext<DeleteTaskMasstransitRequest> context)
    {
        var deleteResult = await service.DeleteTaskAsync(context.Message.Id);

        if (deleteResult.IsSuccess)
            await LogHelper.SendLogRequest(bus, new TaskLogRequest()
            {
                TaskId     = context.Message.Id,
                TaskAction = TaskAction.MarkedAsDeleted,
                SerializedTaskDescription =
                    JsonSerializer.Serialize(("Task {id} marked as deleted", context.Message.Id))
            });
        
        await context.RespondAsync(deleteResult);
    }
}