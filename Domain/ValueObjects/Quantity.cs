using Domain.Common;

public sealed class Quantity : ValueObject
{
    public Quantity()
    {

    }
    public Quantity(decimal quantity)
    {
        Value = quantity;
    }
    public decimal Value { get; init; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static Quantity operator -(Quantity a, Quantity b)
    {
        return a.Value - b.Value;
    }

    public static Quantity operator +(Quantity a, Quantity b)
    {
        return a.Value + b.Value;
    }

    public static implicit operator Quantity(decimal quantity)
    {
        return new Quantity(quantity);
    }

    public static implicit operator decimal(Quantity c)
    {
        return c.Value;
    }

    public static bool operator >(Quantity a, Quantity b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(Quantity a, Quantity b)
    {
        return a.Value < b.Value;
    }


    public static bool operator <=(Quantity a, Quantity b)
    {
        return a.Value <= b.Value;
    }

    public static bool operator >=(Quantity a, Quantity b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator ==(Quantity a, Quantity b)
    {
        return a.Value == b.Value;
    }

    public static bool operator !=(Quantity a, Quantity b)
    {
        return a.Value != b.Value;
    }
    public override bool Equals(object? obj)
    {
        if (!(obj is Quantity))
        {
            return false;
        }

        Quantity quantity = (Quantity)obj;
        return Value == quantity.Value;
    }

    public override int GetHashCode()
    {
        return -5579697 * Value.GetHashCode();
    }
}