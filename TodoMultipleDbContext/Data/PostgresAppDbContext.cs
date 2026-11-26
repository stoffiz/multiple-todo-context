using Microsoft.EntityFrameworkCore;

namespace TodoMultipleDbContext.Data;

public sealed class PostgresAppDbContext(DbContextOptions<PostgresAppDbContext> options) : AppDbContext(options);
