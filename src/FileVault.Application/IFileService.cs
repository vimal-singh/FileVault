using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileVault.Application;

public sealed record FileMetadataDto(
    Guid Id,
    string OriginalFileName,
    string ContentType,
    long FileSize,
    bool IsPublic,
    DateTime? ExpiresAt,
    DateTime CreatedAt);

public sealed record FileDownloadResult(
    string OriginalFileName,
    string ContentType,
    Stream ContentStream);

public interface IFileService
{
    Task<FileMetadataDto> UploadAsync(
        Guid ownerId,
        string originalFileName,
        string contentType,
        Stream contentStream,
        bool isPublic,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default);

    Task<FileDownloadResult?> DownloadAsync(
        Guid fileId,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid fileId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<FileMetadataDto>> ListAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
