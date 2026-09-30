using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileVault.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileVault.Infrastructure.Storage.Local;

public sealed class LocalStorageOptions
{
    public const string SectionName = "StorageSettings:Local";

    /// <summary>Absolute or relative path where files are stored on disk.</summary>
    public string RootPath { get; set; } = "uploads";
}

/// <summary>
/// <see cref="IStorageProvider"/> implementation that persists files to the local filesystem.
/// The storage key is a plain filename (the <see cref="Guid"/> of the file record).
/// </summary>
public sealed class LocalStorageProvider : IStorageProvider
{
    private readonly string _rootPath;
    private readonly ILogger<LocalStorageProvider> _logger;

    public LocalStorageProvider(
        IOptions<LocalStorageOptions> options,
        ILogger<LocalStorageProvider> logger)
    {
        _logger = logger;

        var configured = options.Value.RootPath;
        _rootPath = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(AppContext.BaseDirectory, configured);

        if (!Directory.Exists(_rootPath))
        {
            Directory.CreateDirectory(_rootPath);
            _logger.LogInformation("Created local storage directory: {Path}", _rootPath);
        }
    }

    /// <inheritdoc/>
    public async Task<string> SaveAsync(
        Guid fileId,
        string originalFileName,
        string contentType,
        Stream contentStream,
        CancellationToken cancellationToken = default)
    {
        var storageKey = fileId.ToString("N"); // e.g. "3f2504e04f8941d39a0c0305e82c3301"
        var destinationPath = Path.Combine(_rootPath, storageKey);

        _logger.LogInformation(
            "Saving file {FileName} (id={FileId}) to local path {Path}",
            originalFileName, fileId, destinationPath);

        await using var dest = new FileStream(
            destinationPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81_920,
            useAsync: true);

        await contentStream.CopyToAsync(dest, cancellationToken);

        _logger.LogInformation("File {FileId} saved successfully ({Bytes} bytes)", fileId, dest.Length);
        return storageKey;
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_rootPath, storageKey);
        if (!File.Exists(path))
        {
            _logger.LogWarning("Local file not found for key {Key} at {Path}", storageKey, path);
            return Task.FromResult<Stream?>(null);
        }

        Stream fs = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81_920,
            useAsync: true);

        return Task.FromResult<Stream?>(fs);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_rootPath, storageKey);
        if (File.Exists(path))
        {
            File.Delete(path);
            _logger.LogInformation("Deleted local file at {Path}", path);
        }
        else
        {
            _logger.LogWarning("Delete skipped — local file not found for key {Key}", storageKey);
        }

        return Task.CompletedTask;
    }
}
