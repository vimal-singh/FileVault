using FileVault.Domain.Aggregates.Files;
using FileVault.Domain.Repositories;

namespace FileVault.Application.Features.Files.Commands.UploadFile;

public sealed class UploadFileCommandHandler
{
    private readonly IFileRepository _fileRepository;

    public UploadFileCommandHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<FileAggregate> HandleAsync(UploadFileCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var file = new FileAggregate(
            command.OwnerId,
            command.OriginalFileName,
            new StoragePath(command.StoragePath),
            new ContentType(command.ContentType),
            new FileSize(command.FileSize),
            command.IsPublic,
            command.ExpiresAt);

        await _fileRepository.AddAsync(file, cancellationToken);
        return file;
    }
}
