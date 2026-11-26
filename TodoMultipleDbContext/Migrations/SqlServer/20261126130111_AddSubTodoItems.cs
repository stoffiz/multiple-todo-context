using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoMultipleDbContext.Migrations.SqlServer;

/// <inheritdoc />
public partial class _20261126130111_AddSubTodoItems : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SubTodoItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TodoItemId = table.Column<int>(type: "int", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SubTodoItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_SubTodoItems_Todos_TodoItemId",
                    column: x => x.TodoItemId,
                    principalTable: "Todos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SubTodoItems_TodoItemId",
            table: "SubTodoItems",
            column: "TodoItemId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SubTodoItems");
    }
}
