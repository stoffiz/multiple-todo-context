using Microsoft.EntityFrameworkCore;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Data;

// Abstract: migrations are generated and applied per concrete provider context
// (SqlServerAppDbContext / PostgresAppDbContext) so each gets its own migration
// set and model snapshot, even though they share the same shape here.
public abstract class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();
    public DbSet<SubTodoItem> SubTodoItems => Set<SubTodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
