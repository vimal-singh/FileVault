using FileVault.Domain.Common;

namespace FileVault.Domain.Aggregates.Files;

public class FileAggregate : AggregateRoot
{
    public Guid OwnerId { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public StoragePath StoragePath { get; private set; } = default!;
    public ContentType ContentType { get; private set; } = default!;
    public FileSize FileSize { get; private set; } = default!;
    public FileStatus Status { get; private set; }
    public bool IsPublic { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public List<FilePermission> Permissions { get; private set; } = new();

    protected FileAggregate()
    {
    }

    public FileAggregate(
        Guid ownerId,
        string originalFileName,
        StoragePath storagePath,
        ContentType contentType,
        FileSize fileSize,
        bool isPublic = false,
        DateTime? expiresAt = null)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("Owner id cannot be empty.", nameof(ownerId));

        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException("Original file name cannot be empty.", nameof(originalFileName));

        OwnerId = ownerId;
        OriginalFileName = originalFileName.Trim();
        StoragePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        FileSize = fileSize ?? throw new ArgumentNullException(nameof(fileSize));
        Status = FileStatus.Pending;
        IsPublic = isPublic;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        Permissions = new List<FilePermission>();
    }

    public void MarkAsScanning()
    {
        Status = FileStatus.Scanning;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsClean()
    {
        Status = FileStatus.Clean;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsInfected()
    {
        Status = FileStatus.Infected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        Status = FileStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetVisibility(bool isPublic)
    {
        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPermission(FilePermission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        Permissions.Add(permission);
        UpdatedAt = DateTime.UtcNow;
    }
}
