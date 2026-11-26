using Microsoft.AspNetCore.Http.HttpResults;
using TodoMultipleDbContext.Dtos;
using TodoMultipleDbContext.Services;

namespace TodoMultipleDbContext.Endpoints;

public static class SubTodoItemEndpoints
{
    public static void MapSubTodoItemEndpoints(this IEndpointRouteBuilder app)
    {
        var subTodos = app.MapGroup("/todos/{todoId:int}/subtodos").WithTags("SubTodos");

        subTodos.MapGet("/", async Task<Results<Ok<IReadOnlyList<SubTodoItemResponse>>, NotFound>> (
                int todoId, ISubTodoItemService service, CancellationToken cancellationToken) =>
            {
                if (!await service.TodoExistsAsync(todoId, cancellationToken))
                    return TypedResults.NotFound();

                return TypedResults.Ok(await service.GetAllAsync(todoId, cancellationToken));
            })
            .WithName("GetSubTodoItems")
            .WithSummary("Get all sub-items for a to-do")
            .WithDescription("Returns every sub-item belonging to the given to-do item, or 404 if the to-do doesn't exist.")
            .Produces<IReadOnlyList<SubTodoItemResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        subTodos.MapGet("/{id:int}", async Task<Results<Ok<SubTodoItemResponse>, NotFound>> (
                int todoId, int id, ISubTodoItemService service, CancellationToken cancellationToken) =>
            {
                var subTodo = await service.GetByIdAsync(todoId, id, cancellationToken);
                return subTodo is null ? TypedResults.NotFound() : TypedResults.Ok(subTodo);
            })
            .WithName("GetSubTodoItemById")
            .WithSummary("Get a sub-item by id")
            .WithDescription("Returns a single sub-item, or 404 if the to-do or sub-item doesn't exist.")
            .Produces<SubTodoItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        subTodos.MapPost("/", async Task<Results<Created<SubTodoItemResponse>, NotFound>> (
                int todoId, CreateSubTodoItemRequest request, ISubTodoItemService service, CancellationToken cancellationToken) =>
            {
                if (!await service.TodoExistsAsync(todoId, cancellationToken))
                    return TypedResults.NotFound();

                var created = await service.CreateAsync(todoId, request, cancellationToken);
                return TypedResults.Created($"/todos/{todoId}/subtodos/{created.Id}", created);
            })
            .WithName("CreateSubTodoItem")
            .WithSummary("Create a sub-item")
            .WithDescription("Creates a new sub-item under the given to-do item, or 404 if the to-do doesn't exist.")
            .Produces<SubTodoItemResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound);

        subTodos.MapPut("/{id:int}", async Task<Results<Ok<SubTodoItemResponse>, NotFound>> (
                int todoId, int id, UpdateSubTodoItemRequest request, ISubTodoItemService service, CancellationToken cancellationToken) =>
            {
                var updated = await service.UpdateAsync(todoId, id, request, cancellationToken);
                return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
            })
            .WithName("UpdateSubTodoItem")
            .WithSummary("Update a sub-item")
            .WithDescription("Replaces a sub-item's fields, or 404 if the to-do or sub-item doesn't exist.")
            .Produces<SubTodoItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        subTodos.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (
                int todoId, int id, ISubTodoItemService service, CancellationToken cancellationToken) =>
            {
                var deleted = await service.DeleteAsync(todoId, id, cancellationToken);
                return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("DeleteSubTodoItem")
            .WithSummary("Delete a sub-item")
            .WithDescription("Deletes a sub-item, or 404 if the to-do or sub-item doesn't exist.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
