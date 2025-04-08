using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TaskManager.DAL.DataContext;

namespace TaskManager.DAL.Factory;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaskManagerContext>
{
    public TaskManagerContext CreateDbContext(string[] args)
    {
        Env.Load();
        var connectionString = Env.GetString("CONNECTION_STRING");
        
        var optionsBuilder = new DbContextOptionsBuilder<TaskManagerContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        return new TaskManagerContext(optionsBuilder.Options);
    }
}