namespace Contracts.V1.Responses;

public sealed class SymbolLookupResponse
{
    public required string Symbol { get; init; }
    public required string Description { get; init; }
}