using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser()]
public class BenchmarkUrlCreater
{
    public const string Url = "http/test.url/";
    public string name = "xd";
    [Benchmark()] 
    public void UrlWithParamsStringInterpolation()
    {
        var s = UriCreater.CreateUsingSb(Url , name);
    }
    [Benchmark()] 
    public void UrlWithParamsSpans()
    {
        var s = UriCreater.CreateUsingSpans(Url , name);
    }
    [Benchmark()]
    public void UrlWithParamsStringCreate()
    {
        var s = UriCreater.CreateUsingStringCreate(Url , name);
    }
}