namespace Domain.Auth;

public sealed class IdentifyResult
{
    public required bool IsSuccess { get; init; }
    public required string UserName { get; init; }
}