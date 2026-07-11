namespace FileVault.Application.Features.Files.Commands.UploadFile;

public sealed record UploadFileCommand(
    Guid OwnerId,
    string OriginalFileName,
    string StoragePath,
    string ContentType,
    long FileSize,
    bool IsPublic = false,
    DateTime? ExpiresAt = null);
