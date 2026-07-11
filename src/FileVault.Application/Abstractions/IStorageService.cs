namespace FileVault.Application.Abstractions;

/// <summary>
/// Storage abstraction — defined in Application, implemented in Infrastructure.
/// The domain and application layers never know WHERE files are stored.
/// Switch from Local → Azure Blob → S3 by changing one DI registration.
/// </summary>
public interface IStorageService
{
    Task UploadAsync(Stream stream, string storagePath, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string storagePath, CancellationToken ct = default);
    Task DeleteAsync(string storagePath, CancellationToken ct = default);
    Task<string> GetPresignedUrlAsync(string storagePath, TimeSpan expiry, CancellationToken ct = default);
}
