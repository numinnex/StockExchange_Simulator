using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Options;

public sealed class MatchingEngineOptionsSetup : IConfigureOptions<MatchingEngineOptions>
{
    private readonly IConfiguration _config;

    public MatchingEngineOptionsSetup(IConfiguration config)
    {
        _config = config;
    }
    public void Configure(MatchingEngineOptions options)
    {
        _config.GetSection(nameof(MatchingEngineOptions)).Bind(options);
    }
}