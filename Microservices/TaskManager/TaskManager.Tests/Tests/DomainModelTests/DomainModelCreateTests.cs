using System.Text;
using Ardalis.Result;
using Persistance;
using Shared.Models.TaskManager;
using TaskManager.Domain.Models;
using Xunit;

namespace TaskManager.Tests.Tests.DomainModelTests;

public class DomainModelTests
{
    [Fact]
    public async Task CreateDomainModel_ShoudReturnInvalidResult_IfTitleIsEmpty()
    {
        // arrange
        var id          = Guid.NewGuid();
        var description = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_DESCRIPTION_LENGTH - 1));
        var status      = Helpers.CreateFake<TaskCompletionStatus>();
        var createdAt   = Helpers.CreateFake<DateTime>();
        var updatedAt   = Helpers.CreateFake<DateTime>();
        var deltetedAt  = Helpers.CreateFake<DateTime>();

        var emptyTitle = string.Empty;

        //act
        var modelCreationRes = TaskModel.Create(id, emptyTitle, description, status, createdAt, updatedAt, deltetedAt);


        // assert

        Assert.True(modelCreationRes.IsInvalid());
    }

    [Fact]
    public async Task CreateDomainModel_ShoudReturnInvalidResult_IfDescriptionIsEmpty()
    {
        // arrange
        var id         = Guid.NewGuid();
        var title      = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_TITLE_LENGTH - 1));
        var status     = Helpers.CreateFake<TaskCompletionStatus>();
        var createdAt  = Helpers.CreateFake<DateTime>();
        var updatedAt  = Helpers.CreateFake<DateTime>();
        var deltetedAt = Helpers.CreateFake<DateTime>();

        var emptyDescription = string.Empty;

        // act
        var modelCreationRes = TaskModel.Create(id, title, emptyDescription, status, createdAt, updatedAt, deltetedAt);

        // assert

        Assert.True(modelCreationRes.IsInvalid());
    }

    [Fact]
    public async Task CreateDomainModel_ShoudReturnInvalidResult_IfTitleLengthIsMoreThanLimit()
    {
        // arrange&act
        var id          = Guid.NewGuid();
        var tooBigTitle = GenerateBigString(Limitations.MAX_TITLE_LENGTH);
        var description = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_DESCRIPTION_LENGTH - 1));
        var status      = Helpers.CreateFake<TaskCompletionStatus>();
        var createdAt   = Helpers.CreateFake<DateTime>();
        var updatedAt   = Helpers.CreateFake<DateTime>();
        var deltetedAt  = Helpers.CreateFake<DateTime>();

        var modelCreationRes = TaskModel.Create(id, tooBigTitle, description, status, createdAt, updatedAt, deltetedAt);

        // assert

        Assert.True(modelCreationRes.IsInvalid());
    }

    [Fact]
    public async Task CreateDomainModel_ShoudReturnInvalidResult_IfDescriptionLengthIsMoreThanLimit()
    {
        // arrange
        var id                = Guid.NewGuid();
        var title             = string.Join("", Helpers.CreateFake<string>().Take(Limitations.MAX_TITLE_LENGTH - 1));
        var tooBigDescription = GenerateBigString(Limitations.MAX_DESCRIPTION_LENGTH).ToString();
        var status            = Helpers.CreateFake<TaskCompletionStatus>();
        var createdAt         = Helpers.CreateFake<DateTime>();
        var updatedAt         = Helpers.CreateFake<DateTime>();
        var deltetedAt        = Helpers.CreateFake<DateTime>();

        // act
        var modelCreationRes = TaskModel.Create(id, title, tooBigDescription, status, createdAt, updatedAt, deltetedAt);

        // assert

        Assert.True(modelCreationRes.IsInvalid());
    }

    [Fact]
    public async Task CreateDomainModel_ShoudReturnSuccess_IfValidationIsPassed()
    {
        var id          = Guid.NewGuid();
        var title       = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_TITLE_LENGTH - 1));
        var description = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_DESCRIPTION_LENGTH - 1));
        var status      = Helpers.CreateFake<TaskCompletionStatus>();
        var createdAt   = Helpers.CreateFake<DateTime>();
        var updatedAt   = Helpers.CreateFake<DateTime>();
        var deltetedAt  = Helpers.CreateFake<DateTime>();
        
        // act 
        
        var modelCreationRes = TaskModel.Create(id, title, description, status, createdAt, updatedAt, deltetedAt);
        
        // assert
        
        Assert.True(modelCreationRes.IsSuccess);
    }

    [Fact]
    public async Task ChangeStatusOfDomainModel_ShouldReturnErrorResult_IfInvalidLogic()
    {
        // arrange
        var id          = Guid.NewGuid();
        var title       = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_TITLE_LENGTH - 1));
        var description = string.Join("",Helpers.CreateFake<string>().Take(Limitations.MAX_DESCRIPTION_LENGTH - 1));
        var status      = TaskCompletionStatus.Unhandled;
        var createdAt   = Helpers.CreateFake<DateTime>();
        var updatedAt   = Helpers.CreateFake<DateTime>();
        var deltetedAt  = Helpers.CreateFake<DateTime>();
        
        var unhandledModel = TaskModel.Create(id, title, description, status, createdAt, updatedAt, deltetedAt).Value;
        var inProgressModel = TaskModel.Create(id, title, description, status, createdAt, updatedAt, deltetedAt).Value;
        
        // act

        var unhandledToCompletedModel = unhandledModel.ChangeStatus(TaskCompletionStatus.Completed);
        var inProgressToUnhandledModel = inProgressModel.ChangeStatus(TaskCompletionStatus.Unhandled);
        
        // assert
        
        Assert.True(unhandledToCompletedModel.IsError());
        Assert.True(inProgressToUnhandledModel.IsError());
    }
    
    private string GenerateBigString(int length)
    {
        return new string('a', length + 10);
    }
}