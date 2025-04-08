using Ardalis.Result;
using Shared.Models.TaskManager;
using TaskManager.Domain.Models;

namespace TaskManager.Domain.Interfaces.Services;

public interface ITaskManagerService
{
    
    /// <summary>
    /// Add new task
    /// </summary>
    /// <param name="taskModel">New task model</param>
    /// <returns>Result value of task model</returns>
    public Task<Result<TaskModel>> AddTaskAsync(TaskModel taskModel);
    
    /// <summary>
    /// Change status of the task
    /// </summary>
    /// <c>| Status of the task can be raised up to Completed only in linear ascending order, ex. 0->1, 1->2, bad request for 0->2, 1->0, 2->0 etc</c>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <returns>Result of status changing operation</returns>
    public Task<Result<TaskCompletionStatus>> ChangeTaskStatusAsync(Guid id, TaskCompletionStatus status);
    
    /// <summary>
    /// Get all tasks
    /// </summary>
    /// <returns>Result value of task models list</returns>
    public Task<Result<List<TaskModel>>> GetTasksAsync();
    
    /// <summary>
    /// Mark task as deleted
    /// </summary>
    /// <param name="id">Guid of the task</param>
    /// <returns>Result of deleting operation</returns>
    public Task<Result> DeleteTaskAsync(Guid id);
    
    public Task<Result<(string previousTitle, string previosDescription)>> UpdateTaskAsync(Guid id, string title, string description);
}