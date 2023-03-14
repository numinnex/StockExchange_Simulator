using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Options;

public sealed class JwtSettingsOptionsSetup : IConfigureOptions<JwtSettingsOptions>
{
    private readonly IConfiguration _conifg;

    public JwtSettingsOptionsSetup(IConfiguration conifg)
    {
        _conifg = conifg;
    }


    public void Configure(JwtSettingsOptions options)
    {
        _conifg.GetSection(nameof(JwtSettingsOptions)).Bind(options);
    }
}