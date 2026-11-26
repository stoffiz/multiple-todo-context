using System.ComponentModel.DataAnnotations;

namespace TodoMultipleDbContext.Dtos;

/// <summary>Payload for updating a to-do item. Status is not settable here — it's derived from sub-items.</summary>
public sealed record UpdateTodoItemRequest
{
    [Required, MaxLength(200)]
    public required string Title { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }
}
