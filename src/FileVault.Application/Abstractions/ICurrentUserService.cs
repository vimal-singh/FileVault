namespace FileVault.Application.Abstractions;

/// <summary>
/// Provides the identity of the currently authenticated user.
/// Implemented in Infrastructure using IHttpContextAccessor.
/// Application layer never touches HttpContext directly.
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}
