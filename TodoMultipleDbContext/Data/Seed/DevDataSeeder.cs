using Microsoft.EntityFrameworkCore;
using TodoMultipleDbContext.Models;

namespace TodoMultipleDbContext.Data.Seed;

// Dev-only fixture data. Deterministic (fixed Random seed) so the same 25 to-dos
// show up every time the dev database is recreated, and idempotent — it skips
// seeding if the Todos table already has rows.
public static class DevDataSeeder
{
    private static readonly (string Title, string? Description)[] TodoSeeds =
    [
        ("Plan weekend hiking trip", "Check weather and pack gear"),
        ("Grocery shopping", "Milk, eggs, bread, vegetables"),
        ("Finish Q3 report", "Due Friday, include budget breakdown"),
        ("Book dentist appointment", null),
        ("Learn EF Core migrations", "Study multi-provider setup"),
        ("Clean the garage", null),
        ("Renew passport", "Expires next month"),
        ("Prepare birthday party", "Order cake, send invites"),
        ("Fix leaking faucet", "Call plumber if DIY fails"),
        ("Read \"Clean Code\"", "Chapter 5 onward"),
        ("Update resume", null),
        ("Plan vacation to Japan", "Research flights and hotels"),
        ("Organize home office", null),
        ("Submit tax documents", "Deadline is April 15"),
        ("Set up home gym", "Buy dumbbells and mat"),
        ("Write blog post about Docker", null),
        ("Review pull requests", "Team backlog"),
        ("Call mom", null),
        ("Schedule car maintenance", "Oil change and tire rotation"),
        ("Plan team offsite", "Venue and catering"),
        ("Backup laptop files", null),
        ("Learn Postgres indexing", "Read official docs"),
        ("Water the plants", null),
        ("Refactor authentication module", "Extract to separate service"),
        ("Plan grocery budget for the month", null)
    ];

    private static readonly string[] SubTodoSeeds =
    [
        "Research options", "Make a list", "Set a budget", "Ask for recommendations",
        "Schedule time", "Buy supplies", "Follow up", "Double-check details"
    ];

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Todos.AnyAsync(cancellationToken))
            return;

        var random = new Random(42);
        var now = DateTime.UtcNow;

        for (var i = 0; i < TodoSeeds.Length; i++)
        {
            var (title, description) = TodoSeeds[i];
            var todo = new TodoItem
            {
                Title = title,
                Description = description,
                CreatedAtUtc = now.AddDays(-random.Next(0, 30))
            };

            // Cycle through a fixed set of profiles so every computed TodoItem.Status
            // (NotStarted/InProgress/Completed/Cancelled) actually shows up in the seed
            // data, rather than leaving it to chance.
            foreach (var status in BuildSubTodoStatuses(i % 5, random))
            {
                todo.SubTodos.Add(new SubTodoItem
                {
                    Title = SubTodoSeeds[random.Next(SubTodoSeeds.Length)],
                    Status = status,
                    CreatedAtUtc = todo.CreatedAtUtc.AddHours(random.Next(1, 48))
                });
            }

            db.Todos.Add(todo);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<TodoStatus> BuildSubTodoStatuses(int profile, Random random)
    {
        switch (profile)
        {
            case 0: // no sub-items -> computed NotStarted
                return [];
            case 1: // all NotStarted -> computed NotStarted
                return Enumerable.Repeat(TodoStatus.NotStarted, random.Next(1, 4));
            case 2: // all Completed -> computed Completed
                return Enumerable.Repeat(TodoStatus.Completed, random.Next(1, 4));
            case 3: // all Cancelled -> computed Cancelled
                return Enumerable.Repeat(TodoStatus.Cancelled, random.Next(1, 4));
            default: // genuine mix -> computed InProgress
                var statuses = Enum.GetValues<TodoStatus>();
                var count = random.Next(2, 5);
                return Enumerable.Range(0, count).Select(_ => statuses[random.Next(statuses.Length)]);
        }
    }
}
