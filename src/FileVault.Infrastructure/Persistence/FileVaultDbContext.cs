using FileVault.Domain;
using Microsoft.EntityFrameworkCore;

namespace FileVault.Infrastructure.Persistence;

public sealed class FileVaultDbContext : DbContext
{
    public FileVaultDbContext(DbContextOptions<FileVaultDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FileMetadata> Files => Set<FileMetadata>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<FileMetadata>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.OriginalFileName).IsRequired().HasMaxLength(255);
            entity.Property(f => f.StoragePath).IsRequired();
            entity.Property(f => f.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(f => f.FileSize).IsRequired();
            entity.Property(f => f.OwnerId).IsRequired();
            entity.Property(f => f.IsPublic).IsRequired();
            entity.Property(f => f.CreatedAt).IsRequired();

            entity.HasOne(f => f.Owner)
                .WithMany()
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
