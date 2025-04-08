namespace TaskManager.Domain.Interfaces.Services;

/// <summary>
/// Cleanup job service for deleting Tasks marked to be deleted
/// </summary>
public interface ICleanupService
{
    /// <summary>
    /// Initiates process of cleaning delete marked tasks
    /// </summary>
    /// <returns></returns>
    public Task<Guid[]> Cleanup();
}