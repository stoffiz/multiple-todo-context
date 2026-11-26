# TodoMultipleDbContext

Minimal API + EF Core (Fluent API) with a `TodoItem` model that can run against either SQL Server or PostgreSQL, chosen at startup via environment variables.

## Run a database

```bash
cp .env.example .env    # adjust passwords if you want
docker compose --profile sqlserver up -d   # or:
docker compose --profile postgres up -d    # also starts pgAdmin
```

The `postgres` profile also starts **pgAdmin** at [http://localhost:5050](http://localhost:5050) (log in with `PGADMIN_DEFAULT_EMAIL`/`PGADMIN_DEFAULT_PASSWORD` from `.env`). The `postgres` server is pre-registered (`pgadmin/servers.json`) — just enter the `POSTGRES_PASSWORD` from `.env` the first time you connect to it; pgAdmin doesn't store that password for you. If you change `POSTGRES_USER` in `.env`, update the `Username` in `pgadmin/servers.json` to match.

## Run the app

Set env vars matching the database you started (double underscore maps to nested config), e.g. for Postgres:

```powershell
$env:DB__PROVIDER = "Postgres"
$env:DB__HOST = "localhost"
$env:DB__PORT = "5432"
$env:DB__DATABASE = "tododb"
$env:DB__USER = "todo"
$env:DB__PASSWORD = "todo_password"

dotnet run --project TodoMultipleDbContext
```

Or for SQL Server:

```powershell
$env:DB__PROVIDER = "SqlServer"
$env:DB__HOST = "localhost"
$env:DB__PORT = "1433"
$env:DB__DATABASE = "tododb"
$env:DB__USER = "sa"
$env:DB__PASSWORD = "YourStrong!Passw0rd"

dotnet run --project TodoMultipleDbContext
```

`Provider`, `Host`, `Port`, `Database` default to the values in `appsettings.json` — only `User`/`Password` (and whichever of the others you want to override) need to come from the environment.

The app calls `Database.Migrate()` on startup, applying whichever provider's migrations match the configured `Db:Provider`.

**Dev seed data:** when `ASPNETCORE_ENVIRONMENT` is `Development` (the default for `dotnet run`), `Data/Seed/DevDataSeeder.cs` seeds 25 `TodoItem`s with varied titles/descriptions and a randomized spread of `SubTodoItem`s, deliberately cycling through profiles so all four computed `TodoItem.Status` values (`NotStarted`, `InProgress`, `Completed`, `Cancelled`) show up. It's idempotent (skips seeding if `Todos` already has rows) and uses a fixed `Random` seed, so the same data appears every time you recreate the database. It never runs outside `Development`.

## EF Core migrations

Each provider has its own `DbContext` (`SqlServerAppDbContext`, `PostgresAppDbContext`) and its own migrations folder (`Migrations/SqlServer`, `Migrations/Postgres`). This is required, not just cosmetic: EF Core's migration scaffolder names the model snapshot file after the context's type name, so two providers sharing *one* context type would collide and silently overwrite each other's snapshot the second time you add a migration. Separate context types means separate snapshots, with no manual bookkeeping needed. Both contexts map to the same `TodoItem`/`SubTodoItem` model and Fluent API configuration — only the provider (and naming convention, see below) differs.

**Naming convention:** Postgres's idiomatic style is all-lowercase `snake_case` (so plain SQL doesn't need quoting — `select * from todos` just works). SQL Server's is `PascalCase`. `DbContextConfigurator` applies `.UseSnakeCaseNamingConvention()` ([`EFCore.NamingConventions`](https://github.com/efcore/EFCore.NamingConventions)) only on the Postgres branch, so the *same* Fluent API config (`Data/Configurations/`) produces `todos`/`sub_todo_items`/`created_at_utc` on Postgres and `Todos`/`SubTodoItems`/`CreatedAtUtc` on SQL Server. This only works because table names are **not** set via explicit `.ToTable(...)` calls — EF Core's default convention derives the table name from each `DbSet<T>` property name on `AppDbContext` (hence `DbSet<SubTodoItem> SubTodoItems`, not `SubTodos`), and the naming-convention plugin only rewrites convention-derived names, never an explicit override.

Run all commands from the `TodoMultipleDbContext` project folder. Swap `--context SqlServerAppDbContext` / `-o Migrations/SqlServer` for the Postgres equivalents as needed.

**Add a migration** (after changing the model or Fluent API config):

```bash
dotnet ef migrations add <Name> --context SqlServerAppDbContext -o Migrations/SqlServer
dotnet ef migrations add <Name> --context PostgresAppDbContext  -o Migrations/Postgres
```

**Remove the last (unapplied) migration:**

```bash
dotnet ef migrations remove --context SqlServerAppDbContext
dotnet ef migrations remove --context PostgresAppDbContext
```

**Apply migrations manually** (the app does this automatically on startup, but useful standalone):

```bash
dotnet ef database update --context SqlServerAppDbContext
dotnet ef database update --context PostgresAppDbContext
```

**List migrations / see pending ones:**

```bash
dotnet ef migrations list --context SqlServerAppDbContext
dotnet ef migrations list --context PostgresAppDbContext
```

**Generate a SQL script** (e.g. to hand to a DBA instead of running `Migrate()`):

```bash
dotnet ef migrations script --context SqlServerAppDbContext -o sqlserver-migrate.sql
dotnet ef migrations script --context PostgresAppDbContext  -o postgres-migrate.sql
```

**Drop the database** (dev only — deletes all data):

```bash
dotnet ef database drop --context SqlServerAppDbContext --force
dotnet ef database drop --context PostgresAppDbContext --force
```

These commands read connection settings the same way the app does (`appsettings.json` + `DB__*` env vars), via `Data/DesignTimeDbContextFactories.cs` — no running app or hardcoded connection string needed. Start the relevant docker-compose database first, since `database update`/`drop` need a real connection (`migrations add`/`remove` don't).

## Model

`TodoItem` has a one-to-many relationship with `SubTodoItem` (cascading delete — removing a to-do removes its sub-items). Both share a `Status` enum (`NotStarted`, `InProgress`, `Completed`, `Cancelled`), serialized as a string in JSON.

`SubTodoItem.Status` is a real, stored, independently settable column. `TodoItem.Status` is **not stored** — it's computed on read from its sub-items (`Services/TodoStatusCalculator.cs`):

- no sub-items → `NotStarted`
- all sub-items `NotStarted` → `NotStarted`
- all sub-items `Completed`/`Cancelled` → `Completed` if at least one is `Completed`, otherwise `Cancelled`
- anything else (a mix, or any `InProgress`) → `InProgress`

## Endpoints

Defined in `Endpoints/TodoEndpoints.cs` and `Endpoints/SubTodoItemEndpoints.cs`. See `TodoMultipleDbContext.http` for runnable examples of every endpoint, including error paths.

**To-do items**

- `GET /todos`
- `GET /todos/{id}`
- `POST /todos`
- `PUT /todos/{id}`
- `DELETE /todos/{id}` — also deletes its sub-items

**Sub-items** (nested under their parent to-do; 404 if the to-do doesn't exist)

- `GET /todos/{todoId}/subtodos`
- `GET /todos/{todoId}/subtodos/{id}`
- `POST /todos/{todoId}/subtodos`
- `PUT /todos/{todoId}/subtodos/{id}`
- `DELETE /todos/{todoId}/subtodos/{id}`
