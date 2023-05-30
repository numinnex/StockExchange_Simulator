namespace Application.Common.Models.ReadModels;

public sealed class StockHiLoResponse
{
    public IEnumerable<StockHiLoReadModel> values { get; init; }
}