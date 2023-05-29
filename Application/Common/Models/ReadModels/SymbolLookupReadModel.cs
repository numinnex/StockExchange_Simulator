namespace Application.Common.Models.ReadModels;

public sealed class SymbolLookupReadModel
{
    public required string Symbol { get; init; }
    public required string Exchange { get; init; }
    public required string Instrument_Name { get; init; }
    public required string Country { get; init; }
    public required string Currency { get; init; }
}