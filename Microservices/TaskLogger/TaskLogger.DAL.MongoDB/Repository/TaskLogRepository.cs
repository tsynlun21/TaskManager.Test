using System.Text.Json;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskLogger.DAL.MongoDB.Entity;

namespace TaskLogger.DAL.MongoDB.Repository;

public class TaskLogRepository
{
    private readonly IMongoCollection<TaskLogEntity> _collection;
    
    public TaskLogRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<TaskLogEntity>(settings.Value.CollectionName);
    }

    public async Task InsertAsync(TaskLogEntity taskLogEntity)
    {
        Console.WriteLine(JsonSerializer.Serialize(taskLogEntity));
        await _collection.InsertOneAsync(taskLogEntity);
    }

    public async Task<List<TaskLogEntity>> GetAllAsync()
    {
        return await _collection.Find(filter => true).ToListAsync();
    }
}