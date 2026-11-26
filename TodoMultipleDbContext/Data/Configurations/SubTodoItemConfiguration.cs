using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Data.Configurations;

public class SubTodoItemConfiguration : IEntityTypeConfiguration<SubTodoItem>
{
    public void Configure(EntityTypeBuilder<SubTodoItem> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(s => s.TodoItem)
            .WithMany(t => t.SubTodos)
            .HasForeignKey(s => s.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
