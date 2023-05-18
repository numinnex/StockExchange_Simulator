using ConcurrentHeap;

var comparer = (int x, int y) => x > y;

var minHeap = new ConcurrentHeap<int>(comparer);
minHeap.Enqueue(20);
minHeap.Enqueue(30);
minHeap.Enqueue(50);
minHeap.Enqueue(15);
minHeap.Enqueue(10);
minHeap.Enqueue(16);
minHeap.PrintHeap();

