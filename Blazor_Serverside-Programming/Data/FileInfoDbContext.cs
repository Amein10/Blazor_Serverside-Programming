using Microsoft.EntityFrameworkCore;

namespace Blazor_Serverside_Programming.Data;

public class FileInfoDbContext : DbContext
{
    public FileInfoDbContext(DbContextOptions<FileInfoDbContext> options)
        : base(options)
    {
    }

    public DbSet<FileRecord> Files { get; set; }
}