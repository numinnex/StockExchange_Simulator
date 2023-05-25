namespace Contracts.V1.Requests;

public sealed class IdentifyRequest
{
    public required string Token { get; init; }
}