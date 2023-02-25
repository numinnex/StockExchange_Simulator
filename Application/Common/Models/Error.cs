namespace Application.Common.Models;

public sealed class Error
{
    public static readonly IEnumerable<Error> None = Enumerable.Empty<Error>();
    public required string Code { get; init; }
    public required string Message { get; init; }
    public static implicit operator string(Error error) => error.Code;
    
    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
            return true;
        
        if (a.Message == b.Message && a.Code == b.Code)
            return true;

        return false;
    }
        
    public static bool operator !=(Error? a, Error? b)
    {
        return !(a == b);
    }
}