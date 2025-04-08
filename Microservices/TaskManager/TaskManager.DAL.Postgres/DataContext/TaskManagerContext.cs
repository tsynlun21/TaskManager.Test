using Microsoft.EntityFrameworkCore;
using Persistance;
using TaskManager.DAL.Entities;

namespace TaskManager.DAL.DataContext;

public sealed class TaskManagerContext : DbContext
{
    public DbSet<TaskEntity> TaskEntities { get; init; }

    public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>()
                    .Property(te => te.Description)
                    .HasMaxLength(Limitations.MAX_DESCRIPTION_LENGTH);
        
        modelBuilder.Entity<TaskEntity>()
                    .Property(te => te.Title)
                    .HasMaxLength(Limitations.MAX_TITLE_LENGTH);
        
        base.OnModelCreating(modelBuilder);
    }
}