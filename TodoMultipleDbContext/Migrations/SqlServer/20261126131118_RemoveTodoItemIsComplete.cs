using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoMultipleDbContext.Migrations.SqlServer;

/// <inheritdoc />
public partial class _20261126131118_RemoveTodoItemIsComplete : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsComplete",
            table: "Todos");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsComplete",
            table: "Todos",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }
}
