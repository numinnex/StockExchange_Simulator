namespace Application.Common.Models.ReadModels;

public sealed class StockApiResponse
{
    public required IEnumerable<StockReadModel> Data { get; set; }
    public required string Status { get; set; }
}
    
