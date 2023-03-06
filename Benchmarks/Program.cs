//Notes
// for string params string interpolation method is faster, same memory usage 
// if one of the parameters is of different type than string span method is faster(3x faster apx), same memory usage


using BenchmarkDotNet.Running;
using Benchmarks;


BenchmarkRunner.Run<BenchmarkUrlCreater>();
return;



const string Url = "http/test.url";
string name = "testtrololo";

var sbUrlParams = UriCreater.CreateUsingSb(Url, name);


var spanUrlParams = UriCreater.CreateUsingSpans(Url , name);


Console.WriteLine($"Using Sb url: {sbUrlParams}");
Console.WriteLine($"Using Spans url: {spanUrlParams}");