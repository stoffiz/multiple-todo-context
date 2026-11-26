using Microsoft.EntityFrameworkCore;
using TodoMultipleDbContext.Data;
using TodoMultipleDbContext.Dtos;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Services;

public sealed class SubTodoItemService(AppDbContext db) : ISubTodoItemService
{
    public Task<bool> TodoExistsAsync(int todoId, CancellationToken cancellationToken) =>
        db.Todos.AnyAsync(t => t.Id == todoId, cancellationToken);

    public async Task<IReadOnlyList<SubTodoItemResponse>> GetAllAsync(int todoId, CancellationToken cancellationToken)
    {
        var subTodos = await db.SubTodoItems.AsNoTracking()
            .Where(s => s.TodoItemId == todoId)
            .ToListAsync(cancellationToken);
        return subTodos.Select(ToResponse).ToList();
    }

    public async Task<SubTodoItemResponse?> GetByIdAsync(int todoId, int id, CancellationToken cancellationToken)
    {
        var subTodo = await db.SubTodoItems.AsNoTracking()
            .FirstOrDefaultAsync(s => s.TodoItemId == todoId && s.Id == id, cancellationToken);
        return subTodo is null ? null : ToResponse(subTodo);
    }

    public async Task<SubTodoItemResponse> CreateAsync(int todoId, CreateSubTodoItemRequest request, CancellationToken cancellationToken)
    {
        var subTodo = new SubTodoItem
        {
            TodoItemId = todoId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.SubTodoItems.Add(subTodo);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(subTodo);
    }

    public async Task<SubTodoItemResponse?> UpdateAsync(int todoId, int id, UpdateSubTodoItemRequest request, CancellationToken cancellationToken)
    {
        var subTodo = await db.SubTodoItems.FirstOrDefaultAsync(s => s.TodoItemId == todoId && s.Id == id, cancellationToken);
        if (subTodo is null) return null;

        subTodo.Title = request.Title;
        subTodo.Description = request.Description;
        subTodo.Status = request.Status;
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(subTodo);
    }

    public async Task<bool> DeleteAsync(int todoId, int id, CancellationToken cancellationToken)
    {
        var subTodo = await db.SubTodoItems.FirstOrDefaultAsync(s => s.TodoItemId == todoId && s.Id == id, cancellationToken);
        if (subTodo is null) return false;

        db.SubTodoItems.Remove(subTodo);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static SubTodoItemResponse ToResponse(SubTodoItem subTodo) =>
        new(subTodo.Id, subTodo.TodoItemId, subTodo.Title, subTodo.Description, subTodo.Status,
            new DateTimeOffset(subTodo.CreatedAtUtc, TimeSpan.Zero));
}
