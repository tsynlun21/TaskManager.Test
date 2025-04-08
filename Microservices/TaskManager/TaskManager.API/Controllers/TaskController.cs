using Ardalis.Result;
using Infrustructure.Masstransit;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Persistance.Masstransit;
using Shared.Contracts.TaskManager;
using Shared.Masstransit.TaskManager.Requests;
using TaskManager.Domain.Interfaces.Services;
using TaskManager.Domain.Models;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController(ITaskManagerService service, IBusControl busControl) : ControllerBase
{
    private readonly Uri _rabbitMqUri = new Uri($"queue:{RabbitQueueNames.TASK_MANAGER_QUEUE}");

    /// <summary>
    /// Request to add new task
    /// </summary>
    /// <param name="contract">Contract for adding task</param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> AddTask([FromBody] AddTaskContract contract)
    {
        var res = await ClientResponseWorker.GetRabbitMessageResponse<AddTaskMasstransitRequest, Result<TaskModelDTO>>(
            new AddTaskMasstransitRequest(contract), busControl, _rabbitMqUri);

        if (res.IsError())
            return BadRequest(res);

        return Ok(res.Value);
    }

    /// <summary>
    /// Request to change status of the task
    /// </summary>
    /// <param name="contract">Contract for changing status</param>
    /// <returns></returns>
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ChangeStatus([FromQuery] ChangeStatusContract contract)
    {
        var opRes = await ClientResponseWorker.GetRabbitMessageResponse<ChangeStatusMasstransitRequest, Result>(
            new ChangeStatusMasstransitRequest(contract.TaskId, contract.Status), busControl, _rabbitMqUri);

        if (opRes.IsError())
        {
            return BadRequest(opRes.Errors);
        }

        if (opRes.IsNotFound())
        {
            return NotFound(opRes.Errors);
        }

        return Ok();
    }

    /// <summary>
    /// Request to retrieve all tasks
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetTasks()
    {
        var opRes = await ClientResponseWorker
           .GetRabbitMessageResponse<GetTasksMasstransitRequest, Result<TaskModelDTO[]>>(
                new GetTasksMasstransitRequest(), busControl, _rabbitMqUri);

        if (opRes.IsNoContent())
        {
            return NoContent();
        }

        return Ok(opRes.Value);
    }

    /// <summary>
    /// Request to delete task
    /// </summary>
    /// <param name="contract">Contract for marking task as deleted</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteTask([FromQuery] DeleteTaskContract contract)
    {
        var opRes = await ClientResponseWorker.GetRabbitMessageResponse<DeleteTaskMasstransitRequest, Result>(
            new DeleteTaskMasstransitRequest(contract.TaskId), busControl, _rabbitMqUri);

        if (opRes.IsNotFound())
            return NotFound(opRes.Errors);

        return Accepted();
    }

    /// <summary>
    /// Request to update info of the task
    /// </summary>
    /// <param name="contract">Contract for changing task info</param>
    /// <returns></returns>
    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskInfoContract contract)
    {
        var opRes = await ClientResponseWorker.GetRabbitMessageResponse<UpdateTaskInfoMasstransitRequest, Result>(
            new UpdateTaskInfoMasstransitRequest(contract.TaskId, contract.Title, contract.Description), busControl,
            _rabbitMqUri);
        
        if (opRes.IsNotFound())
            return NotFound(opRes.Errors);
        
        if (opRes.IsInvalid())
            return BadRequest(opRes.Errors);
        
        return Ok();
    }
}