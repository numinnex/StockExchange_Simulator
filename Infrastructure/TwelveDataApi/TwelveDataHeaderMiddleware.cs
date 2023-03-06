using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.TwelveDataApi;

public sealed class TwelveDataHeaderMiddleware : DelegatingHandler
{
    private readonly IOptions<TwelveDataApiOptions> _options;

    public TwelveDataHeaderMiddleware(IOptions<TwelveDataApiOptions> options)
    {
        _options = options;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-RapidAPI-Key", _options.Value.Key);
        request.Headers.Add("X-RapidAPI-Host", _options.Value.Host);

        return base.SendAsync(request, cancellationToken);
    }
}