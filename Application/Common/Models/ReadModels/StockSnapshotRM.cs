namespace Application.Common.Models.ReadModels;

public sealed class StockSnapshotRM
{
    public double Close { get; set; } 
    public DateTime DateTime { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Open { get; set; }
}