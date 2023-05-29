namespace Application.Common.Models.ReadModels;

public sealed class SymbolLookupApiResponse
{
    public required IEnumerable<SymbolLookupReadModel> Data { get; set; }
    public required string Status { get; set; }
}