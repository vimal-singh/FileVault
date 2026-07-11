using FileVault.Domain.Aggregates.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileVault.Infrastructure.Persistence.Configurations;

public sealed class FileConfiguration : IEntityTypeConfiguration<FileAggregate>
{
    public void Configure(EntityTypeBuilder<FileAggregate> builder)
    {
        builder.ToTable("Files");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerId).IsRequired();
        builder.Property(x => x.OriginalFileName).HasMaxLength(512).IsRequired();
        builder.Property(x => x.StoragePath)
            .HasConversion(
                v => v.Value,
                v => new StoragePath(v))
            .HasMaxLength(1024)
            .IsRequired();
        builder.Property(x => x.ContentType)
            .HasConversion(
                v => v.Value,
                v => new ContentType(v))
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.FileSize)
            .HasConversion(
                v => v.Bytes,
                v => new FileSize(v))
            .IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.IsPublic).IsRequired();
        builder.Property(x => x.ExpiresAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.Ignore(x => x.DomainEvents);

        builder.OwnsMany(x => x.Permissions, permissionBuilder =>
        {
            permissionBuilder.ToTable("FilePermissions");
            permissionBuilder.HasKey(p => p.Id);
            permissionBuilder.Property(p => p.UserId).IsRequired();
            permissionBuilder.Property(p => p.Permission).IsRequired();
            permissionBuilder.Property(p => p.GrantedAt).IsRequired();
        });

        builder.HasIndex(x => x.OwnerId).HasDatabaseName("IX_Files_OwnerId");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Files_Status");
    }
}
