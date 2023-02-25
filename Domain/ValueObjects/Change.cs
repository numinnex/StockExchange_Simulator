using Domain.Common;

namespace Domain.ValueObjects;

public sealed class Change : ValueObject
{
    public double Value { get; internal set; }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}