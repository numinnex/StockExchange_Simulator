namespace Domain.Auth;

public sealed class AuthenticationResult
{
    public string? Token { get; init; } 
    public required bool Success { get; init; }
    public IEnumerable<string>? Errors { get; init; }
    public string? RefreshToken { get; init; } 
}