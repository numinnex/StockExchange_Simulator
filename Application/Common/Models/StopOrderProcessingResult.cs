namespace Application.Common.Models;

public sealed class StopOrderProcessingResult
{
    public required decimal FilledCount { get; init; } 
    public required int MatchedCount { get; init; } 
    public required decimal CostAccumulate { get; init; } 
    
    public static StopOrderProcessingResult Default => new StopOrderProcessingResult() { FilledCount = 0, MatchedCount = 0, CostAccumulate = 0 };
}