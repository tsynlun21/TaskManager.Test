using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models.TaskManager;

namespace TaskLogger.DAL.MongoDB.Entity;

public class TaskLogEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid TaskId { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public TaskAction ActionType { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public string TaskData {get; init; }
}