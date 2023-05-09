using Domain.Auth;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthenticationResult> RegisterAsync(string requestEmail, string requestPassword);
    Task<AuthenticationResult> LoginAsync(string email, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> UserExistsAsync(string userId, CancellationToken token);
}