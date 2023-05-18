namespace Application.Common.Models;

public sealed class StopOrderProcessingResult
{
    public required int FilledCount { get; init; } 
    public required int MatchedCount { get; init; } 
    public required decimal CostAccumulate { get; init; } 
}