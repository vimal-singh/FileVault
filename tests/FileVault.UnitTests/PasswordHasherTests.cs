using FileVault.Infrastructure.Auth;
using Xunit;

namespace FileVault.UnitTests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldReturnHashedString()
    {
        // Arrange
        var password = "SecurePassword123";

        // Act
        var hash = PasswordHasher.Hash(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void Verify_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var password = "MySecretPassword";
        var hash = PasswordHasher.Hash(password);

        // Act
        var result = PasswordHasher.Verify(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var password = "MySecretPassword";
        var wrongPassword = "WrongPassword";
        var hash = PasswordHasher.Hash(password);

        // Act
        var result = PasswordHasher.Verify(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }
}
