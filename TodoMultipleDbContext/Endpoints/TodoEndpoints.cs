using Microsoft.AspNetCore.Http.HttpResults;
using TodoMultipleDbContext.Dtos;
using TodoMultipleDbContext.Services;

namespace TodoMultipleDbContext.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var todos = app.MapGroup("/todos").WithTags("Todos");

        todos.MapGet("/", async (ITodoService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetTodoItems")
            .WithSummary("Get all to-do items")
            .WithDescription("Returns every to-do item.")
            .Produces<IReadOnlyList<TodoItemResponse>>(StatusCodes.Status200OK);

        todos.MapGet("/{id:int}", async Task<Results<Ok<TodoItemResponse>, NotFound>> (
                int id, ITodoService service, CancellationToken cancellationToken) =>
            {
                var todo = await service.GetByIdAsync(id, cancellationToken);
                return todo is null ? TypedResults.NotFound() : TypedResults.Ok(todo);
            })
            .WithName("GetTodoItemById")
            .WithSummary("Get a to-do item by id")
            .WithDescription("Returns a single to-do item, or 404 if it doesn't exist.")
            .Produces<TodoItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        todos.MapPost("/", async (CreateTodoItemRequest request, ITodoService service, CancellationToken cancellationToken) =>
            {
                var created = await service.CreateAsync(request, cancellationToken);
                return TypedResults.Created($"/todos/{created.Id}", created);
            })
            .WithName("CreateTodoItem")
            .WithSummary("Create a to-do item")
            .WithDescription("Creates a new to-do item.")
            .Produces<TodoItemResponse>(StatusCodes.Status201Created);

        todos.MapPut("/{id:int}", async Task<Results<Ok<TodoItemResponse>, NotFound>> (
                int id, UpdateTodoItemRequest request, ITodoService service, CancellationToken cancellationToken) =>
            {
                var updated = await service.UpdateAsync(id, request, cancellationToken);
                return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
            })
            .WithName("UpdateTodoItem")
            .WithSummary("Update a to-do item")
            .WithDescription("Replaces a to-do item's fields, or 404 if it doesn't exist.")
            .Produces<TodoItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        todos.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (
                int id, ITodoService service, CancellationToken cancellationToken) =>
            {
                var deleted = await service.DeleteAsync(id, cancellationToken);
                return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("DeleteTodoItem")
            .WithSummary("Delete a to-do item")
            .WithDescription("Deletes a to-do item and cascades to its sub-items, or 404 if it doesn't exist.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
