using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FileVault.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileVault.API.Controllers;

/// <summary>Multipart/form-data model for the file upload endpoint.</summary>
public sealed class UploadRequest
{
    public IFormFile File { get; set; } = null!;
    public bool IsPublic { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
}

[ApiController]
[Route("api/v1/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload(
        [FromForm] UploadRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("No file was uploaded.");

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        await using var stream = request.File.OpenReadStream();
        var result = await _fileService.UploadAsync(
            userId.Value,
            request.File.FileName,
            request.File.ContentType,
            stream,
            request.IsPublic,
            request.ExpiresAt,
            cancellationToken);

        return CreatedAtAction(nameof(Download), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var result = await _fileService.DownloadAsync(id, userId, cancellationToken);
        if (result == null)
            return NotFound("File not found, expired, or you do not have permission to download it.");

        return File(result.ContentStream, result.ContentType, result.OriginalFileName, enableRangeProcessing: true);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var success = await _fileService.DeleteAsync(id, userId.Value, cancellationToken);
        if (!success)
            return NotFound("File not found or you are not authorized to delete it.");

        return NoContent();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var files = await _fileService.ListAsync(userId.Value, page, pageSize, cancellationToken);
        return Ok(files);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : null;
    }
}
