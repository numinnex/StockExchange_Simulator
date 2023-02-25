using Domain.Common;
using Domain.Entities;

namespace Domain.ValueObjects;

public sealed class StockPosition : ValueObject
{
    public required Stock Stock { get; set; }
    public required int Quantity { get; set; }
    
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Stock;
        yield return Quantity;
    }
}