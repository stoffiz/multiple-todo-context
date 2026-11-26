namespace TodoMultipleDbContext.Data;

public enum DatabaseProvider
{
    SqlServer,
    Postgres
}

public class DbSettings
{
    public const string SectionName = "Db";

    public string Provider { get; set; } = nameof(DatabaseProvider.Postgres);
    public string Host { get; set; } = "localhost";
    public int Port { get; set; }
    public string Database { get; set; } = "tododb";
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public DatabaseProvider ResolveProvider() =>
        Enum.TryParse<DatabaseProvider>(Provider, ignoreCase: true, out var result)
            ? result
            : throw new InvalidOperationException(
                $"Unknown database provider '{Provider}'. Expected '{nameof(DatabaseProvider.SqlServer)}' or '{nameof(DatabaseProvider.Postgres)}'.");
}
