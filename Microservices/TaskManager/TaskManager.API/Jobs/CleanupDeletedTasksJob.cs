using System.Text.Json;
using MassTransit;
using Quartz;
using Shared.Masstransit.TaskLogger.Requests;
using Shared.Models.TaskManager;
using TaskManager.API.Masstransit.Consumers;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.Jobs;

public class CleanupDeletedTasksJob(ICleanupService service, ILogger<CleanupDeletedTasksJob> logger, IBus bus) : IJob
{
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("CleanupDeletedTasksJob started");
        var deletedIds = await service.Cleanup();
        logger.LogInformation("CleanupDeletedTasksJob finished, cleaned {amount}", deletedIds.Length);
        
        foreach (var deletedId in deletedIds)
        {
            await LogHelper.SendLogRequest(bus, new TaskLogRequest()
            {
                TaskId                    = deletedId,
                TaskAction                = TaskAction.Deleted,
                SerializedTaskDescription = "Task deleted from db",
            });
        }
        
    }
}