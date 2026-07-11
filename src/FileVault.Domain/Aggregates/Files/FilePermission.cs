namespace FileVault.Domain.Aggregates.Files;

public sealed class FilePermission
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Permission { get; private set; }
    public DateTime GrantedAt { get; private set; }

    protected FilePermission()
    {
        Id = Guid.NewGuid();
        Permission = string.Empty;
    }

    public FilePermission(Guid userId, string permission, DateTime? grantedAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission cannot be empty.", nameof(permission));

        Id = Guid.NewGuid();
        UserId = userId;
        Permission = permission.Trim();
        GrantedAt = grantedAt ?? DateTime.UtcNow;
    }
}
