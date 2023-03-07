using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public record Person(string name, int age);

[MemoryDiagnoser()]
public class ListEnumerableBenchmark
{

    public static List<int> list = Enumerable.Range(1, 500).ToList();
    public static IEnumerable<int> numerable = Enumerable.Range(1, 500);


    public ListEnumerableBenchmark()
    {
    }

    [Benchmark()]
    public void IterateList()
    {
        foreach (var l in list)
        {
        }
    }
    [Benchmark()]
    public void IterateNumerable()
    {
        using (var seq = numerable.GetEnumerator())
        {
            while(seq.MoveNext()){}
        }
    }
}