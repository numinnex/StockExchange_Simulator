namespace Contracts.V1.Responses;

public sealed class AuthFailResponse
{
    public IEnumerable<string> Errors { get; init; }
}