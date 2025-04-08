using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models.TaskManager;
using TaskManager.Domain.Models;

namespace TaskManager.DAL.Entities;

[Table("Tasks")]
public class TaskEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    [Column("Status")]
    public TaskCompletionStatus CompletionStatus { get; set; }

    [Required]
    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}