using TodoMultipleDbContext.Dtos;

namespace TodoMultipleDbContext.Services;

public interface ISubTodoItemService
{
    Task<bool> TodoExistsAsync(int todoId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubTodoItemResponse>> GetAllAsync(int todoId, CancellationToken cancellationToken);
    Task<SubTodoItemResponse?> GetByIdAsync(int todoId, int id, CancellationToken cancellationToken);
    Task<SubTodoItemResponse> CreateAsync(int todoId, CreateSubTodoItemRequest request, CancellationToken cancellationToken);
    Task<SubTodoItemResponse?> UpdateAsync(int todoId, int id, UpdateSubTodoItemRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int todoId, int id, CancellationToken cancellationToken);
}
