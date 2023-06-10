namespace Contracts.V1.Responses;

public sealed class PortfolioResponse
{
    public required List<ValueSnapshotResponse> ValueSnapshots { get; init; } 
    public required decimal TotalValue { get; init; } 
}    
