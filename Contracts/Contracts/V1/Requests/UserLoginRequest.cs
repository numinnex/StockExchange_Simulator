namespace Contracts.V1.Requests;

public sealed class UserLoginRequest
{
    public required string Email { get; init; }
    public string Password { get; init; } 
}