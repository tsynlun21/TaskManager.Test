using TaskLogger.Domain.Models;

namespace TaskLogger.Domain.Services;

public interface ITaskLoggerService
{
    public Task LogTask(TaskLogModel model);

    public Task<TaskLogModel[]> GetAll();
}