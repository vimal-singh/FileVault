using FileVault.Domain.Aggregates.Files;
using FileVault.Domain.Repositories;

namespace FileVault.Application.Features.Files.Queries.GetFileById;

public sealed class GetFileByIdQueryHandler
{
    private readonly IFileRepository _fileRepository;

    public GetFileByIdQueryHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<FileAggregate?> HandleAsync(GetFileByIdQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await _fileRepository.GetByIdAsync(query.FileId, cancellationToken);
    }
}
