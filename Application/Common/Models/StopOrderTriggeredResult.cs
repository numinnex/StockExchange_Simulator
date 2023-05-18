namespace Application.Common.Models;

public sealed class StopOrderTriggeredResult
{
    public required bool IsFilled { get; init; }
    public required bool IsMatched { get; init; }
    public required decimal Cost { get; init; }
}