namespace Application.Common.Models;

public sealed class StopOrderProcessingResult
{
    public required decimal FilledCount { get; init; } 
    public required int MatchedCount { get; init; } 
    public required decimal CostAccumulate { get; init; } 
}