namespace Contracts.V1.Requests;

public sealed class SymbolLookupRequest
{
    public required string Symbol { get; init; }
}