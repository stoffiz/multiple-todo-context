using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TodoMultipleDbContext.Data;
using TodoMultipleDbContext.Data.Seed;
using TodoMultipleDbContext.Endpoints;
using TodoMultipleDbContext.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ISubTodoItemService, SubTodoItemService>();

var dbSettings = builder.Configuration.GetSection(DbSettings.SectionName).Get<DbSettings>() ?? new DbSettings();

switch (dbSettings.ResolveProvider())
{
    case DatabaseProvider.SqlServer:
        builder.Services.AddDbContext<SqlServerAppDbContext>(options => DbContextConfigurator.Configure(options, dbSettings));
        builder.Services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<SqlServerAppDbContext>());
        break;
    case DatabaseProvider.Postgres:
        builder.Services.AddDbContext<PostgresAppDbContext>(options => DbContextConfigurator.Configure(options, dbSettings));
        builder.Services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<PostgresAppDbContext>());
        break;
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    app.Logger.LogInformation("Using database provider: {Provider} ({Host}:{Port}/{Database})",
        dbSettings.Provider, dbSettings.Host, dbSettings.Port, dbSettings.Database);
    db.Database.Migrate();

    if (app.Environment.IsDevelopment())
    {
        await DevDataSeeder.SeedAsync(db, CancellationToken.None);
    }
}

app.MapTodoEndpoints();
app.MapSubTodoItemEndpoints();

app.Run();
