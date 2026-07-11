using FileVault.Domain.Aggregates.Files;

namespace FileVault.Domain.Repositories;

/// <summary>
/// Repository interface for FileAggregate.
/// Defined in Domain — implemented in Infrastructure.
/// Follows the Repository pattern: one repository per Aggregate Root.
/// </summary>
public interface IFileRepository
{
    Task<FileAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FileAggregate>> GetByOwnerIdAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(FileAggregate file, CancellationToken ct = default);
    Task UpdateAsync(FileAggregate file, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
