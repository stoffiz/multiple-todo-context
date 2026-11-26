using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TodoMultipleDbContext.Data;

public static class DbContextConfigurator
{
    public static void Configure(DbContextOptionsBuilder optionsBuilder, DbSettings settings)
    {
        switch (settings.ResolveProvider())
        {
            case DatabaseProvider.SqlServer:
                optionsBuilder.UseSqlServer(BuildSqlServerConnectionString(settings));
                break;
            case DatabaseProvider.Postgres:
                optionsBuilder.UseNpgsql(BuildPostgresConnectionString(settings));
                optionsBuilder.UseSnakeCaseNamingConvention();
                break;
        }
    }

    private static string BuildSqlServerConnectionString(DbSettings settings)
    {
        var port = settings.Port == 0 ? 1433 : settings.Port;
        return new SqlConnectionStringBuilder
        {
            DataSource = $"{settings.Host},{port}",
            InitialCatalog = settings.Database,
            UserID = settings.User,
            Password = settings.Password,
            TrustServerCertificate = true
        }.ConnectionString;
    }

    private static string BuildPostgresConnectionString(DbSettings settings)
    {
        var port = settings.Port == 0 ? 5432 : settings.Port;
        return new NpgsqlConnectionStringBuilder
        {
            Host = settings.Host,
            Port = port,
            Database = settings.Database,
            Username = settings.User,
            Password = settings.Password
        }.ConnectionString;
    }
}
