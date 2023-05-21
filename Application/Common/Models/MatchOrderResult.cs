namespace Application.Common.Models;

public sealed class MatchOrderResult
{
    public required bool IsMatched { get; init; }    
    public required bool IsFilled { get; init; }    
    public decimal? Cost { get; init; }
}