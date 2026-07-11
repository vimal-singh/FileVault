using FileVault.Application.Features.Files.Commands.UploadFile;
using FileVault.Application.Features.Files.Queries.GetFileById;
using FileVault.Domain.Aggregates.Files;
using FileVault.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FileVault.API.Controllers.v1;

[ApiController]
[Route("api/v1/files")]
public sealed class FilesController : ControllerBase
{
    private readonly UploadFileCommandHandler _uploadFileHandler;
    private readonly GetFileByIdQueryHandler _getFileByIdHandler;

    public FilesController(
        UploadFileCommandHandler uploadFileHandler,
        GetFileByIdQueryHandler getFileByIdHandler)
    {
        _uploadFileHandler = uploadFileHandler;
        _getFileByIdHandler = getFileByIdHandler;
    }

    [HttpPost]
    public async Task<ActionResult<FileResponse>> Upload([FromBody] UploadFileRequest request, CancellationToken cancellationToken)
    {
        var command = new UploadFileCommand(
            request.OwnerId,
            request.OriginalFileName,
            request.StoragePath,
            request.ContentType,
            request.FileSize,
            request.IsPublic,
            request.ExpiresAt);

        var file = await _uploadFileHandler.HandleAsync(command, cancellationToken);
        return Ok(new FileResponse(file.Id, file.OriginalFileName, file.Status.ToString(), file.StoragePath.Value));
    }

    [HttpGet("{fileId:guid}")]
    public async Task<ActionResult<FileResponse>> GetById(Guid fileId, CancellationToken cancellationToken)
    {
        var file = await _getFileByIdHandler.HandleAsync(new GetFileByIdQuery(fileId), cancellationToken);
        if (file is null)
            return NotFound();

        return Ok(new FileResponse(file.Id, file.OriginalFileName, file.Status.ToString(), file.StoragePath.Value));
    }

    public sealed record UploadFileRequest(
        Guid OwnerId,
        string OriginalFileName,
        string StoragePath,
        string ContentType,
        long FileSize,
        bool IsPublic = false,
        DateTime? ExpiresAt = null);

    public sealed record FileResponse(
        Guid Id,
        string OriginalFileName,
        string Status,
        string StoragePath);
}
