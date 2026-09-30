using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileVault.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileVault.Infrastructure.Storage.Azure;

public sealed class AzureBlobStorageOptions
{
    public const string SectionName = "StorageSettings:Azure";

    /// <summary>Azure Storage account connection string.</summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>Blob container name (will be created if absent).</summary>
    public string ContainerName { get; set; } = "filevault";
}

/// <summary>
/// <see cref="IStorageProvider"/> implementation backed by Azure Blob Storage.
/// The storage key is the blob name, which equals the file's <see cref="Guid"/> (no extension).
/// </summary>
public sealed class AzureBlobStorageProvider : IStorageProvider
{
    private readonly BlobContainerClient _container;
    private readonly ILogger<AzureBlobStorageProvider> _logger;

    public AzureBlobStorageProvider(
        IOptions<AzureBlobStorageOptions> options,
        ILogger<AzureBlobStorageProvider> logger)
    {
        _logger = logger;

        var opts = options.Value;

        if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            throw new InvalidOperationException(
                $"Azure Blob Storage connection string is not configured. " +
                $"Set '{AzureBlobStorageOptions.SectionName}:ConnectionString' in your app settings.");

        var serviceClient = new BlobServiceClient(opts.ConnectionString);
        _container = serviceClient.GetBlobContainerClient(opts.ContainerName);
    }

    /// <inheritdoc/>
    public async Task<string> SaveAsync(
        Guid fileId,
        string originalFileName,
        string contentType,
        Stream contentStream,
        CancellationToken cancellationToken = default)
    {
        await _container.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        var blobName = fileId.ToString("N");
        var blobClient = _container.GetBlobClient(blobName);

        _logger.LogInformation(
            "Uploading blob {BlobName} for file {OriginalFileName} (type={ContentType})",
            blobName, originalFileName, contentType);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType,
                ContentDisposition = $"attachment; filename=\"{originalFileName}\""
            },
            Metadata =
            {
                ["OriginalFileName"] = originalFileName,
                ["FileId"] = fileId.ToString()
            }
        };

        await blobClient.UploadAsync(contentStream, uploadOptions, cancellationToken);

        _logger.LogInformation("Blob {BlobName} uploaded successfully", blobName);
        return blobName;
    }

    /// <inheritdoc/>
    public async Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var blobClient = _container.GetBlobClient(storageKey);

        try
        {
            var response = await blobClient.OpenReadAsync(
                new BlobOpenReadOptions(allowModifications: false),
                cancellationToken);

            _logger.LogInformation("Opened read stream for blob {BlobName}", storageKey);
            return response;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogWarning("Blob {BlobName} not found in container", storageKey);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var blobClient = _container.GetBlobClient(storageKey);
        var deleted = await blobClient.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);

        if (deleted.Value)
            _logger.LogInformation("Deleted blob {BlobName}", storageKey);
        else
            _logger.LogWarning("Delete skipped — blob {BlobName} did not exist", storageKey);
    }
}
