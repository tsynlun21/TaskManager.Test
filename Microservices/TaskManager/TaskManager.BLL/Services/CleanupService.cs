using Microsoft.EntityFrameworkCore;
using TaskManager.DAL.DataContext;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.BLL.Services;

public class CleanupService(TaskManagerContext context) : ICleanupService
{
    public async Task<Guid[]> Cleanup()
    {
        var toBeDeleted = await context.TaskEntities.Where(x => x.DeletedAt != null).ToListAsync();
        
        
        var ids = toBeDeleted.Select(x => x.Id).ToArray();

        if (toBeDeleted.Any())
        {
            context.TaskEntities.RemoveRange(toBeDeleted);
            await context.SaveChangesAsync();
        }
        
        return ids;
    }
}