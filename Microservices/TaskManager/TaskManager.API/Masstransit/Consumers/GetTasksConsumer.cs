using Ardalis.Result;
using MassTransit;
using Shared.Masstransit.TaskManager.Requests;
using TaskManager.DAL.Converters;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Masstransit.Consumers;

public class GetTasksConsumer(ITaskManagerService service) : IConsumer<GetTasksMasstransitRequest>
{
    public async Task Consume(ConsumeContext<GetTasksMasstransitRequest> context)
    {
        var getTasksResult = await service.GetTasksAsync();

        if (getTasksResult.IsSuccess)
        {
            var mappedResult = getTasksResult.Map(x => x.Select(tm => tm.ToTaskModelDTO()).ToArray());
            await context.RespondAsync(mappedResult);
        }
            
        
        await context.RespondAsync(getTasksResult);
    }
}