namespace Application.Common.Models;

public sealed class OrderProcessingResult
{
    public required bool IsMatched { get; init; }    
    public required bool IsFilled { get; init; }    
    public decimal? Cost { get; init; }    
    
    public static OrderProcessingResult Default => new OrderProcessingResult() { IsMatched = false, IsFilled = false };
}