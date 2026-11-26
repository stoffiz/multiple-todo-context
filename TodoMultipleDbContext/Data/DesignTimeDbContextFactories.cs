using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TodoMultipleDbContext.Data;

// Used by `dotnet ef` so migrations can be generated/applied without running Program.cs
// (which would otherwise call Database.Migrate() against a real connection). The provider
// is implied by which --context is targeted, so these only need Host/Port/Database/User/Password.
internal static class DesignTimeSettings
{
    public static DbSettings Load()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetSection(DbSettings.SectionName).Get<DbSettings>() ?? new DbSettings();
    }
}

public class SqlServerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SqlServerAppDbContext>
{
    public SqlServerAppDbContext CreateDbContext(string[] args)
    {
        var settings = DesignTimeSettings.Load();
        settings.Provider = nameof(DatabaseProvider.SqlServer);

        var optionsBuilder = new DbContextOptionsBuilder<SqlServerAppDbContext>();
        DbContextConfigurator.Configure(optionsBuilder, settings);

        return new SqlServerAppDbContext(optionsBuilder.Options);
    }
}

public class PostgresDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PostgresAppDbContext>
{
    public PostgresAppDbContext CreateDbContext(string[] args)
    {
        var settings = DesignTimeSettings.Load();
        settings.Provider = nameof(DatabaseProvider.Postgres);

        var optionsBuilder = new DbContextOptionsBuilder<PostgresAppDbContext>();
        DbContextConfigurator.Configure(optionsBuilder, settings);

        return new PostgresAppDbContext(optionsBuilder.Options);
    }
}
