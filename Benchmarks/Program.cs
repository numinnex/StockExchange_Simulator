// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Benchmarks;


Console.WriteLine("Hello, World!");
BenchmarkRunner.Run<BenchmarkUrlCreater>();
return;

const string Url = "http/test.url";
int Id = 6;

var sbUrlParams = UriCreater.CreateUsingSb(Url, Id);


var spanUrlParams = UriCreater.CreateUsingSpans(Url , Id);


Console.WriteLine($"Using Sb url: {sbUrlParams}");
Console.WriteLine($"Using Spans url: {spanUrlParams}");
