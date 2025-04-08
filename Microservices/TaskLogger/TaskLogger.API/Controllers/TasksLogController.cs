using Microsoft.AspNetCore.Mvc;
using Shared.Models.TaskManager;
using TaskLogger.Domain.Models;
using TaskLogger.Domain.Services;

namespace TaskLogger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksLogController(ITaskLoggerService service) : ControllerBase
{
    /// <summary>
    /// Get all tasks logs
    /// </summary>
    /// <returns>List of logged tasks</returns>
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Get()
    {
        var models = await service.GetAll();
        
        return Ok(models);
    } 
}