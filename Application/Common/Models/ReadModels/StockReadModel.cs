namespace Application.Common.Models.ReadModels;

public sealed class StockReadModel
{
    public required string Symbol { get; set; } 
    public required string Name { get; set; } 
    public required string Currency { get; set; } 
    public required string Exchange { get; set; } 
    public required string Mic_Code { get; set; } 
    public required string Country { get; set; } 
    public required string Type { get; set; } 
}