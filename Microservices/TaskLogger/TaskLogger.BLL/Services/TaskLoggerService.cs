using System.Text.Json;
using TaskLogger.DAL.MongoDB.Converters;
using TaskLogger.DAL.MongoDB.Repository;
using TaskLogger.Domain.Models;
using TaskLogger.Domain.Services;

namespace TaskLogger.BLL.Services;

public class TaskLoggerService(TaskLogRepository repository) : ITaskLoggerService
{
    public async Task LogTask(TaskLogModel model)
    {
        var entity = model.ToTaskLogEntity();
        Console.WriteLine(JsonSerializer.Serialize(entity));
        await repository.InsertAsync(entity);
    }

    public async Task<TaskLogModel[]> GetAll()
    {
        var res = (await repository.GetAllAsync()).Select(x => x.ToTaskLogModel()).ToArray();
        return res;
    }
}