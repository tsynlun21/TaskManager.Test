using System.Text.Json;
using Ardalis.Result;
using Infrustructure.Masstransit;
using MassTransit;
using Shared.Masstransit.TaskLogger.Requests;
using Shared.Masstransit.TaskManager.Requests;
using Shared.Models.TaskManager;
using TaskManager.DAL.Converters;
using TaskManager.Domain.Interfaces.Services;
using TaskModel = TaskManager.Domain.Models.TaskModel;

namespace TaskManager.API.Masstransit.Consumers;

public class AddTaskConsumer(ITaskManagerService service, IBus bus) : IConsumer<AddTaskMasstransitRequest>
{
    private readonly Uri rabbitLogTaskUri = new Uri($"queue:{RabbitQueueNames.TASK_LOGGER_QUEUE}");
    
    public async Task Consume(ConsumeContext<AddTaskMasstransitRequest> context)
    {
        var createModelRes = TaskModel.Create(Guid.NewGuid(), context.Message.Contract.Title, context.Message.Contract.Description,
            TaskCompletionStatus.Unhandled, DateTime.UtcNow, null, null);
        
        if (createModelRes.IsInvalid())
            await context.RespondAsync(createModelRes);
        
        var taskAddResult = await service.AddTaskAsync(createModelRes.Value);

        await LogHelper.SendLogRequest(bus, new TaskLogRequest()
        {
            TaskId                    = taskAddResult.Value.Id,
            TaskAction                = TaskAction.Created,
            SerializedTaskDescription = JsonSerializer.Serialize(taskAddResult.Value)
        });

        var res = taskAddResult.Map(x => x.ToTaskModelDTO());
        
        await context.RespondAsync(res);
    }
}