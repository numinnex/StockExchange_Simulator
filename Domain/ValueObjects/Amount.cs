using Domain.Common;

public class Amount : ValueObject
{
    public Amount(decimal amount)
    {
        Value = amount;
    }
    public decimal Value { get; set; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator Amount(decimal amount)
    {
        return new Amount(amount);
    }

    public static implicit operator decimal(Amount c)
    {
        return c.Value;
    }

    public int CompareTo(Amount other)
    {
        return Value.CompareTo(other.Value);
    }

    public bool Equals(Amount other)
    {
        return Value == other.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is Amount && ((Amount)obj).Value == Value;
    }

    public override int GetHashCode()
    {
        return 92834 + Value.GetHashCode();
    }

    public static bool operator <(Amount left, Amount right)
    {
        return left.Value < right.Value;
    }

    public static bool operator <=(Amount left, Amount right)
    {
        return left.Value <= right.Value;
    }

    public static bool operator >(Amount left, Amount right)
    {
        return left.Value > right.Value;
    }

    public static bool operator >=(Amount left, Amount right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator ==(Amount left, Amount right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(Amount left, Amount right)
    {
        return left.Value != right.Value;
    }

}