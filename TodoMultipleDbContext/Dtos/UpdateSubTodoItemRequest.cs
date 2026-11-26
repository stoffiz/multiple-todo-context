using System.ComponentModel.DataAnnotations;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Dtos;

/// <summary>Payload for updating a sub-item.</summary>
public sealed record UpdateSubTodoItemRequest
{
    [Required, MaxLength(200)]
    public required string Title { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }

    public required TodoStatus Status { get; init; }
}
