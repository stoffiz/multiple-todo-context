using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Services;

// Derives a TodoItem's overall status from its SubTodoItems, rather than storing it.
public static class TodoStatusCalculator
{
    public static TodoStatus Compute(IReadOnlyCollection<SubTodoItem> subTodos)
    {
        if (subTodos.Count == 0)
            return TodoStatus.NotStarted;

        if (subTodos.All(s => s.Status == TodoStatus.NotStarted))
            return TodoStatus.NotStarted;

        if (subTodos.All(s => s.Status is TodoStatus.Completed or TodoStatus.Cancelled))
            return subTodos.Any(s => s.Status == TodoStatus.Completed) ? TodoStatus.Completed : TodoStatus.Cancelled;

        return TodoStatus.InProgress;
    }
}
