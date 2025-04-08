using Ardalis.Result;
using MassTransit;
using Shared.Masstransit.TaskLogger.Requests;
using TaskLogger.Domain.Models;
using TaskLogger.Domain.Services;

namespace TaskLogger.API.Consumers;

public class LogActionConsumer(ILogger<LogActionConsumer> logger, ITaskLoggerService service) : IConsumer<TaskLogRequest>
{
    public async Task Consume(ConsumeContext<TaskLogRequest> context)
    {
        var createRes = TaskLogModel.Create(context.Message.TaskId, context.Message.TaskAction,
            context.Message.SerializedTaskDescription);

        if (createRes.IsInvalid())
            logger.LogError("Error when creating task log. Errors: {errors}", createRes.ValidationErrors);
        
        await service.LogTask(createRes);
        logger.LogInformation("Task log {id} added to data base", createRes.Value.Id);
    }
}