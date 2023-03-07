using Domain.Common;

namespace Domain.ValueObjects;

public sealed class Price : ValueObject
{
    public double Value { get; init; }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}