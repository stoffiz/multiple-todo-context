using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Dtos;

/// <summary>Represents a to-do item. <see cref="Status"/> is derived from its sub-items, not stored.</summary>
public sealed record TodoItemResponse(
    int Id,
    string Title,
    string? Description,
    TodoStatus Status,
    DateTimeOffset CreatedAtUtc);
