using FileVault.Domain.Aggregates.Files;
using FileVault.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FileVault.Infrastructure.Persistence.Repositories;

public sealed class FileRepository : IFileRepository
{
    private readonly FileVaultDbContext _dbContext;

    public FileRepository(FileVaultDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FileAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Files
            .Include(f => f.Permissions)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IEnumerable<FileAggregate>> GetByOwnerIdAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default)
    {
        return await _dbContext.Files
            .Where(f => f.OwnerId == ownerId)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task AddAsync(FileAggregate file, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        await _dbContext.Files.AddAsync(file, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FileAggregate file, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        _dbContext.Files.Update(file);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Files.AnyAsync(f => f.Id == id, ct);
    }
}
