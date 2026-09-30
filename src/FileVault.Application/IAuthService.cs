using System.Threading.Tasks;

namespace FileVault.Application;

public interface IAuthService
{
    Task<bool> RegisterAsync(string username, string password);
    Task<string?> LoginAsync(string username, string password);
}
