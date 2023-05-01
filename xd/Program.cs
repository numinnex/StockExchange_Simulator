using ConcurrentHeap;

// var client = new HttpClient();
// var request = new HttpRequestMessage
// {
//     Method = HttpMethod.Get,
//     RequestUri = new Uri("https://twelve-data1.p.rapidapi.com/stocks?country=United%20Kingdom&format=json"),
//     Headers =
//     {
//         { "X-RapidAPI-Key", "cc3a2376aamsh10265419dca0ff4p108edcjsn4f8d4d1df821" },
//         { "X-RapidAPI-Host", "twelve-data1.p.rapidapi.com" }
//     },
// };
// var secondRequest = new HttpRequestMessage()
// {
//     Method = HttpMethod.Get,
//     RequestUri =
//         new Uri("https://twelve-data1.p.rapidapi.com/time_series?symbol=AMZN&interval=1day&outputsize=30&format=json"),
//     Headers =
//     {
//         { "X-RapidAPI-Key", "cc3a2376aamsh10265419dca0ff4p108edcjsn4f8d4d1df821" },
//         { "X-RapidAPI-Host", "twelve-data1.p.rapidapi.com" },
//     },
// };
// using (var response = await client.SendAsync(request))
// {
//     response.EnsureSuccessStatusCode();
//     var body = await response.Content.ReadFromJsonAsync<StockApiResponse>();
//     Console.WriteLine();
// }

// using (var response = await client.SendAsync(secondRequest))
// {
//     response.EnsureSuccessStatusCode();
//     var body = await response.Content.ReadFromJsonAsync<TimeSeriesResponse>();

// internal class Program
// {
//     private static void Main(string[] args)
//     {
//         Func<int, int, bool> minHeapComparer = (x, y) => x > y;
//         var minHeap = new ConcurrentHeap<int>(minHeapComparer);
//         minHeap.Enqueue(20);
//         minHeap.Enqueue(30);
//         minHeap.Enqueue(50);
//         minHeap.Enqueue(15);
//         minHeap.Enqueue(10);
//         minHeap.Enqueue(16);
//         minHeap.PrintHeap();
//         minHeap.PrintHeap();
//     }
// }