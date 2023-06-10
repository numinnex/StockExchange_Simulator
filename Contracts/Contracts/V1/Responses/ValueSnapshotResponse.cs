namespace Contracts.V1.Responses;

public sealed class ValueSnapshotResponse
{
    public DateTimeOffset Timestamp { get; set; } 
    public decimal Value { get; set; }
}