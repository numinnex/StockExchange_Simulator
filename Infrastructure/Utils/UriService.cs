using Application.Common.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Utils;

public sealed class UriService : IUriService
{
    private readonly string _baseUri;
    public UriService(string baseUri)
    {
        _baseUri = baseUri;
    }

    public Uri GetAllActiveMarketOrders(int pageNumber, int pageSize)
    {
        var uri = new Uri(_baseUri);
        if (pageNumber < 1 || pageSize < 1)
        {
            return uri;
        }

        var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pageNumber.ToString());
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pageSize.ToString());
        return new Uri(modifiedUri);
    }
}