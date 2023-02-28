using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser()]
public class BenchmarkUrlCreater
{
    public const string Url = "http/test.url/";
    public int Id = 6;
    [Benchmark()] 
    public void UrlWithParamsSb()
    {
        var s = UriCreater.CreateUsingSb(Url , Id);
    }
    [Benchmark()] 
    public void UrlWithParamsSpans()
    {
        var s = UriCreater.CreateUsingSpans(Url , Id);
    }
}