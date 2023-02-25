using Domain.Common;

namespace Domain.ValueObjects;

public sealed class Price : ValueObject
{
    public double Value { get; internal set; }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}