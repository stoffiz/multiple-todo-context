using Microsoft.EntityFrameworkCore;
using TodoMultipleDbContext.Data;
using TodoMultipleDbContext.Dtos;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Services;

public sealed class TodoService(AppDbContext db) : ITodoService
{
    public async Task<IReadOnlyList<TodoItemResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var todos = await db.Todos.AsNoTracking().Include(t => t.SubTodos).ToListAsync(cancellationToken);
        return todos.Select(ToResponse).ToList();
    }

    public async Task<TodoItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await db.Todos.AsNoTracking().Include(t => t.SubTodos)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return todo is null ? null : ToResponse(todo);
    }

    public async Task<TodoItemResponse> CreateAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(todo);
    }

    public async Task<TodoItemResponse?> UpdateAsync(int id, UpdateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var todo = await db.Todos.Include(t => t.SubTodos).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (todo is null) return null;

        todo.Title = request.Title;
        todo.Description = request.Description;
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(todo);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (todo is null) return false;

        db.Todos.Remove(todo);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TodoItemResponse ToResponse(TodoItem todo) =>
        new(todo.Id, todo.Title, todo.Description, TodoStatusCalculator.Compute(todo.SubTodos),
            new DateTimeOffset(todo.CreatedAtUtc, TimeSpan.Zero));
}
