using TaskLogger.DAL.MongoDB.Entity;
using TaskLogger.Domain.Models;

namespace TaskLogger.DAL.MongoDB.Converters;

public static class TaskLogConverters
{
    public static TaskLogModel ToTaskLogModel(this TaskLogEntity entity)
    {
        var taskModel = TaskLogModel.Create(entity.TaskId, entity.ActionType, entity.TaskData);
        return taskModel;
    }

    public static TaskLogEntity ToTaskLogEntity(this TaskLogModel model)
    {
        return new TaskLogEntity()
        {
            Id = Guid.NewGuid(),
            TaskId     = model.Id,
            ActionType = model.TaskAction,
            TaskData   = model.SerializedTaskDescription
        };
    }
}