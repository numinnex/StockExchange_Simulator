
namespace Benchmarks;

public sealed class UriCreater
{
    public static string CreateUsingSb(string Url, string name)
    {
        //StringBuilder urlWithParameters = new();
        //urlWithParameters.Append(Url);
        //urlWithParameters.Append($"/{id}");
        //return urlWithParameters.ToString();
        return $"{Url}/{name}";
    }

    public static string CreateUsingSpans(ReadOnlySpan<char> Url, ReadOnlySpan<char> name)
    {
        Span<char> urlWithParameters = stackalloc char[Url.Length + name.Length + 1];
        
        Url.CopyTo(urlWithParameters);
        urlWithParameters[Url.Length] = '/';
        name.CopyTo(urlWithParameters.Slice(Url.Length + 1));

        return urlWithParameters.ToString();
    }
    public static string CreateUsingStringCreate(string Url, string name)
    {
        const string test ="xd";
        return string.Create(Url.Length + name.Length + 1 , Url , (span, value) =>
        {
            value.AsSpan().CopyTo(span);
            span[value.Length  ] = '/';
            test.AsSpan().CopyTo(span[(value.Length + 1)..]);
        });
    }
    
    
}