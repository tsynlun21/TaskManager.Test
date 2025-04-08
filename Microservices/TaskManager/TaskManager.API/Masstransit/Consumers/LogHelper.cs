using Infrustructure.Masstransit;
using MassTransit;
using Shared.Masstransit.TaskLogger.Requests;

namespace TaskManager.API.Masstransit.Consumers;

/// <summary>
/// Helping class to produce log message
/// </summary>
public static class LogHelper
{
    private static readonly Uri rabbitLogTaskUri = new Uri($"queue:{RabbitQueueNames.TASK_LOGGER_QUEUE}");

    /// <summary>
    /// Produces message to log queue
    /// </summary>
    /// <param name="bus"></param>
    /// <param name="request"></param>
    public static async Task SendLogRequest(IBus bus, TaskLogRequest request)
    {
        var endpoint = await bus.GetSendEndpoint(rabbitLogTaskUri);
        await endpoint.Send(request);
    }
}