namespace TodoMultipleDbContext.Models;

public class SubTodoItem
{
    public int Id { get; set; }
    public int TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoStatus Status { get; set; } = TodoStatus.NotStarted;
    public DateTime CreatedAtUtc { get; set; }
}
