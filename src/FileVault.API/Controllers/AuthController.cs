using System.Threading.Tasks;
using FileVault.Application;
using Microsoft.AspNetCore.Mvc;

namespace FileVault.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        var success = await _authService.RegisterAsync(request.Username, request.Password);
        if (!success)
            return BadRequest("Username already exists or invalid registration request.");

        return Ok("Registration successful.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        var token = await _authService.LoginAsync(request.Username, request.Password);
        if (token == null)
            return Unauthorized("Invalid username or password.");

        return Ok(new LoginResponse(token));
    }

    public sealed record RegisterRequest(string Username, string Password);
    public sealed record LoginRequest(string Username, string Password);
    public sealed record LoginResponse(string Token);
}
