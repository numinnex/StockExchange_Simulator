namespace Infrastructure.Options;

public sealed class JwtSettingsOptions
{
    public required string Secret { get; init; }
    public required TimeSpan TokenLifetime { get; init; }
}