using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Options;

public sealed class TwelveDataApiOptionSetup : IConfigureOptions<TwelveDataApiOptions>
{
    private readonly IConfiguration _config;

    public TwelveDataApiOptionSetup(IConfiguration config)
    {
        _config = config;
    }
    
    public void Configure(TwelveDataApiOptions options)
    {
        _config.GetSection(nameof(TwelveDataApiOptions))
            .Bind(options);
    }
}