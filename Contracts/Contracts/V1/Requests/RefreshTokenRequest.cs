namespace Contracts.V1.Requests;

public sealed class RefreshTokenRequest
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
}