using Application.Common.Interfaces;

namespace Infrastructure.DateTimeService;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}