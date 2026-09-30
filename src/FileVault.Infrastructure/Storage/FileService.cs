using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileVault.Application;
using FileVault.Domain;
using FileVault.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileVault.Infrastructure.Storage;

/// <summary>
/// Orchestrates file-management use cases by coordinating between the
/// <see cref="FileVaultDbContext"/> (metadata) and an <see cref="IStorageProvider"/> (bytes).
/// </summary>
public sealed class FileService : IFileService
{
    private readonly FileVaultDbContext _dbContext;
    private readonly IStorageProvider _storage;
    private readonly ILogger<FileService> _logger;

    public FileService(
        FileVaultDbContext dbContext,
        IStorageProvider storage,
        ILogger<FileService> logger)
    {
        _dbContext = dbContext;
        _storage   = storage;
        _logger    = logger;
    }

    /// <inheritdoc/>
    public async Task<FileMetadataDto> UploadAsync(
        Guid ownerId,
        string originalFileName,
        string contentType,
        Stream contentStream,
        bool isPublic,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default)
    {
        var fileId = Guid.NewGuid();

        _logger.LogInformation(
            "Uploading {FileName} ({ContentType}) for owner {OwnerId}",
            originalFileName, contentType, ownerId);

        // Delegate byte storage to the provider; receive an opaque key
        var storageKey = await _storage.SaveAsync(
            fileId,
            originalFileName,
            contentType,
            contentStream,
            cancellationToken);

        var metadata = new FileMetadata
        {
            Id               = fileId,
            OriginalFileName = Path.GetFileName(originalFileName),
            StoragePath      = storageKey,
            ContentType      = contentType,
            FileSize         = contentStream.CanSeek ? contentStream.Position : 0,
            OwnerId          = ownerId,
            IsPublic         = isPublic,
            ExpiresAt        = expiresAt,
            CreatedAt        = DateTime.UtcNow
        };

        await _dbContext.Files.AddAsync(metadata, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "File {FileName} saved with ID {FileId} (key={StorageKey})",
            originalFileName, fileId, storageKey);

        return ToDto(metadata);
    }

    /// <inheritdoc/>
    public async Task<FileDownloadResult?> DownloadAsync(
        Guid fileId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Download requested: FileId={FileId} UserId={UserId}", fileId, userId);

        var metadata = await _dbContext.Files.FindAsync(
            new object[] { fileId }, cancellationToken);

        if (metadata is null)
        {
            _logger.LogWarning("Download failed — file {FileId} not found", fileId);
            return null;
        }

        if (metadata.ExpiresAt.HasValue && metadata.ExpiresAt.Value < DateTime.UtcNow)
        {
            _logger.LogWarning("Download failed — file {FileId} has expired", fileId);
            return null;
        }

        if (!metadata.IsPublic && metadata.OwnerId != userId)
        {
            _logger.LogWarning(
                "Download denied — user {UserId} does not own private file {FileId}",
                userId, fileId);
            return null;
        }

        var stream = await _storage.OpenReadAsync(metadata.StoragePath, cancellationToken);
        if (stream is null)
        {
            _logger.LogError(
                "Storage object missing for file {FileId} (key={Key})",
                fileId, metadata.StoragePath);
            return null;
        }

        return new FileDownloadResult(metadata.OriginalFileName, metadata.ContentType, stream);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(
        Guid fileId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Delete requested: FileId={FileId} UserId={UserId}", fileId, userId);

        var metadata = await _dbContext.Files.FindAsync(
            new object[] { fileId }, cancellationToken);

        if (metadata is null)
        {
            _logger.LogWarning("Delete failed — file {FileId} not found", fileId);
            return false;
        }

        if (metadata.OwnerId != userId)
        {
            _logger.LogWarning(
                "Delete denied — user {UserId} does not own file {FileId}",
                userId, fileId);
            return false;
        }

        await _storage.DeleteAsync(metadata.StoragePath, cancellationToken);

        _dbContext.Files.Remove(metadata);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("File {FileId} deleted", fileId);
        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FileMetadataDto>> ListAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Listing files for user {UserId} (page={Page}, size={PageSize})",
            userId, page, pageSize);

        return await _dbContext.Files
            .Where(f => f.OwnerId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => ToDto(f))
            .ToListAsync(cancellationToken);
    }

    // -------------------------------------------------------------------------

    private static FileMetadataDto ToDto(FileMetadata f) =>
        new(f.Id, f.OriginalFileName, f.ContentType, f.FileSize, f.IsPublic, f.ExpiresAt, f.CreatedAt);
}
