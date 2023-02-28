namespace Infrastructure.Options;

public sealed class TwelveDataApiOptions
{
    public required string Uri { get; init; }
    public required string Key { get; init; }
    public required string Host { get; init; }
}