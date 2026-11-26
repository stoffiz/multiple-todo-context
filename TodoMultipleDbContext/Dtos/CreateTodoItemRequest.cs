using System.ComponentModel.DataAnnotations;

namespace TodoMultipleDbContext.Dtos;

/// <summary>Payload for creating a to-do item.</summary>
public sealed record CreateTodoItemRequest
{
    [Required, MaxLength(200)]
    public required string Title { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }
}
