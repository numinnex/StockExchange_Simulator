using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Text;

namespace Benchmarks;

public sealed class UriCreater
{
    public static string CreateUsingSb(string Url, int id)
    {
        StringBuilder urlWithParameters = new();
        urlWithParameters.Append(Url);
        urlWithParameters.Append($"/{id}");
        return urlWithParameters.ToString();
    }

    public static string CreateUsingSpans(ReadOnlySpan<char> Url, int id)
    {
        Span<char> urlWithParameters = stackalloc char[Url.Length + 2];
        
        Url.CopyTo(urlWithParameters);
        urlWithParameters[Url.Length] = '/';
        id.TryFormat(urlWithParameters.Slice(Url.Length  + 1), out _);

        return urlWithParameters.ToString();
    }
    
    
}