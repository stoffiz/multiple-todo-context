using Microsoft.EntityFrameworkCore;

namespace TodoMultipleDbContext.Data;

public sealed class SqlServerAppDbContext(DbContextOptions<SqlServerAppDbContext> options) : AppDbContext(options);
