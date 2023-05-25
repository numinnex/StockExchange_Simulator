namespace Infrastructure.Options;

public sealed class JwtSettingsOptions
{
    public string Secret { get; init; }
    public TimeSpan TokenLifetime { get; init; }
}