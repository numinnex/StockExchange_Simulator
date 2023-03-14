namespace Contracts.V1.Responses;

public sealed class AuthSuccesResponse
{
    public required string Token { get;init; }
    public required string RefreshToken { get; init; }
}