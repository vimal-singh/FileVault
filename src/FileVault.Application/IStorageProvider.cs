using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileVault.Application;

/// <summary>
/// Abstraction over the underlying binary storage back-end.
/// Implementations may target local disk, Azure Blob Storage, S3, etc.
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Persists a stream and returns an opaque storage key that can be used
    /// to retrieve or delete the object later.
    /// </summary>
    Task<string> SaveAsync(
        Guid fileId,
        string originalFileName,
        string contentType,
        Stream contentStream,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a readable stream for the given storage key.
    /// Returns <c>null</c> if the object does not exist.
    /// </summary>
    Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the object identified by <paramref name="storageKey"/>.
    /// Does not throw if the object is already absent.
    /// </summary>
    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}
