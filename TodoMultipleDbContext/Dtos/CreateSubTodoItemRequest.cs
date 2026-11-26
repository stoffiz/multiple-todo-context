using System.ComponentModel.DataAnnotations;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Dtos;

/// <summary>Payload for creating a sub-item under a to-do item.</summary>
public sealed record CreateSubTodoItemRequest
{
    [Required, MaxLength(200)]
    public required string Title { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }

    public TodoStatus Status { get; init; } = TodoStatus.NotStarted;
}
