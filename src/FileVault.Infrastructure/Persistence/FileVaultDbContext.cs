using FileVault.Domain.Aggregates.Files;
using FileVault.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FileVault.Infrastructure.Persistence;

public sealed class FileVaultDbContext : DbContext
{
    public FileVaultDbContext(DbContextOptions<FileVaultDbContext> options) : base(options)
    {
    }

    public DbSet<FileAggregate> Files => Set<FileAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
