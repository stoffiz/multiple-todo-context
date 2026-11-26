using TodoMultipleDbContext.Dtos;

namespace TodoMultipleDbContext.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItemResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TodoItemResponse> CreateAsync(CreateTodoItemRequest request, CancellationToken cancellationToken);
    Task<TodoItemResponse?> UpdateAsync(int id, UpdateTodoItemRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
