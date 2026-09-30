using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FileVault.Application;
using FileVault.Domain;
using FileVault.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FileVault.Infrastructure.Auth;

public sealed class AuthService : IAuthService
{
    private readonly FileVaultDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(FileVaultDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var existingUser = await _dbContext.Users.AnyAsync(u => u.Username == username);
        if (existingUser)
            return false;

        var hashedPassword = PasswordHasher.Hash(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username.Trim(),
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
            return null;

        if (!PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var keyString = jwtSettings["Key"] ?? "ThisIsASecretKeyForFileVault2026!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "FileVaultAPI",
            audience: jwtSettings["Audience"] ?? "FileVaultUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
