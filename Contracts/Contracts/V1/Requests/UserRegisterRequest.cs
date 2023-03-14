namespace Contracts.V1.Requests;

public sealed class UserRegisterRequest
{
    public required string Email { get; init; } 
    public required string Password { get; init; } 
}