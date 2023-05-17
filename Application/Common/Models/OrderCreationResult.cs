namespace Application.Common.Models;

public sealed class OrderCreationResult
{
    public required bool IsMatched { get; init; }    
    public required bool IsFilled { get; init; }    
    public decimal? Cost { get; init; }    
    public int BidLevelsCount { get; init; }
    public int AskLevelsCount { get; init; }
}