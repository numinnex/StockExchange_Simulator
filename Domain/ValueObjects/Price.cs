using Domain.Common;

namespace Domain.ValueObjects;

public sealed class Price : ValueObject, IComparable<Price>
{
    public Price()
    {

    }
    public Price(decimal price)
    {
        Value = price;
    }
    public decimal Value { get; init; }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static Price operator -(Price a, Price b)
    {
        return a.Value - b.Value;
    }

    public static Price operator +(Price a, Price b)
    {
        return a.Value + b.Value;
    }

    public static implicit operator Price(decimal price)
    {
        return new Price(price);
    }

    public static implicit operator decimal(Price c)
    {
        return c.Value;
    }

    public static bool operator >(Price a, Price b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(Price a, Price b)
    {
        return a.Value < b.Value;
    }


    public static bool operator <=(Price a, Price b)
    {
        return a.Value <= b.Value;
    }

    public static bool operator >=(Price a, Price b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator ==(Price a, Price b)
    {
        return a.Value == b.Value;
    }

    public static bool operator !=(Price a, Price b)
    {
        return a.Value != b.Value;
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is Price))
        {
            return false;
        }

        Price price = (Price)obj;
        return Value == price.Value;
    }

    public override int GetHashCode()
    {
        return -5579697 * Value.GetHashCode();
    }

    public int CompareTo(Price? other)
    {
        if (this.Value > other!.Value)
        {
            return 1;
        }
        if (this.Value < other.Value)
        {
            return -1;
        }
        return 0;
    }
}
