using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Dtos;

/// <summary>Represents a sub-item belonging to a to-do item.</summary>
public sealed record SubTodoItemResponse(
    int Id,
    int TodoItemId,
    string Title,
    string? Description,
    TodoStatus Status,
    DateTimeOffset CreatedAtUtc);
