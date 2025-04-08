using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManager.DAL.DataContext;

namespace TaskManager.Tests;

internal static class Helpers
{
    private static Fixture _fixture = new Fixture();

    internal static DbContextOptions<TaskManagerContext> CreateInMemoryDbContextOptions()
    {
        return new DbContextOptionsBuilder<TaskManagerContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    internal static T CreateFake<T>()
    {
        return _fixture.Create<T>();
    }
}