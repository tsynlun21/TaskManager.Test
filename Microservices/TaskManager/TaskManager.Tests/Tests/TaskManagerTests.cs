using Ardalis.Result;
using NSubstitute.Core;
using Shared.Models.TaskManager;
using TaskManager.BLL.Services;
using TaskManager.DAL.DataContext;
using Xunit;

namespace TaskManager.Tests.Tests;

public class TaskManagerTests
{
    [Fact]
    public async Task ChangeTaskStatus_ShouldReturnNotFoundResult_WhenTaskNotFound()
    {
        // arrange

        var opts = Helpers.CreateInMemoryDbContextOptions();
        
        await using var context = new TaskManagerContext(opts);
        
        var service = new TaskManagerService(context, false);

        var anyStatus = Helpers.CreateFake<TaskCompletionStatus>();
        var anyGuid = Helpers.CreateFake<Guid>();
        
        // act
        var result    = await service.ChangeTaskStatusAsync(anyGuid, anyStatus);
        
        // assert
        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task GetTasks_ShouldReturnNoContentResult_WhenNoTasks()
    {
        // arrange

        var opts = Helpers.CreateInMemoryDbContextOptions();
        
        await using var context = new TaskManagerContext(opts);
        
        var service = new TaskManagerService(context, false);
        
        // act
        var result    = await service.GetTasksAsync();
        
        // assert
        Assert.True(result.IsNoContent());
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFoundResult_WhenTaskNotFound()
    {
        // arrange

        var opts = Helpers.CreateInMemoryDbContextOptions();
        
        await using var context = new TaskManagerContext(opts);
        
        var service = new TaskManagerService(context, false);
        
        var anyGuid   = Helpers.CreateFake<Guid>();
        
        // act
        var result    = await service.DeleteTaskAsync(anyGuid);
        
        // assert
        Assert.True(result.IsNotFound());
    }
}