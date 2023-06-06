using System.Runtime.Serialization;
using Domain.Common;

namespace Domain.Entities;

public sealed class ValueSnapshot : SubEntity
{
    public Portfolio? Portfolio { get; set; }
    public DateTimeOffset Timestamp { get; set; } 
    public decimal Value { get; set; }
}