namespace ConcurrentHeap;

public sealed class ConcurrentHeap<T>
{
    private readonly object _sync = new object();
    
    private List<T> _heap;
    private readonly Func<T, T, bool> _compare;
    private int Count
    {
        get
        {
            lock (_sync)
            {
                return _heap.Count;
            }
        }
    }
    public ConcurrentHeap(Func<T, T, bool> compare)
    {
        _heap = new List<T>();
        _compare = compare;
    }
    public void Enqueue(T item)
    {
        lock (_sync)
        {
            _heap.Add(item);
            HeapifyUp(Count - 1);
        }
    }

    public T Pop()
    {
        if (Count == 0)
        {
        }

        T result;
        lock (_sync)
        {
            result = _heap[0];
            _heap[0] = _heap[Count - 1];
            _heap.RemoveAt(Count - 1);
            HeapifyDown(0);
        }

        return result;
    }
    public void PrintHeap()
    {
        Console.WriteLine(string.Join(", ", _heap)); 
    }
    private void HeapifyDown(int idx)
    {
        var leftIdx = LeftChildIndex(idx);
        var rightIdx = RightChildIndex(idx);

        if (idx >= Count || leftIdx >= Count)
        {
            return;
        }

        var value = _heap[idx];

        var leftValue = _heap[leftIdx];
        var rightValue = _heap[rightIdx];
        if (_compare(leftValue, rightValue) && _compare(value, rightValue))
        {
            _heap[idx] = rightValue;
            _heap[rightIdx] = value;
            HeapifyDown(rightIdx); 
        }
        else if(_compare(rightValue, leftValue) && _compare(value, leftValue))
        {
            _heap[idx] = leftValue;
            _heap[leftIdx] = value;
            HeapifyDown(leftIdx); 
        }
    }

    private void HeapifyUp(int idx)
    {
        if (idx == 0)
        {
            return;
        }
        var parent = ParentIndex(idx);
        var parentValue = _heap[parent];
        var value = _heap[idx];

        if (_compare(parentValue, value))
        {
            _heap[idx] = parentValue;
            _heap[parent] = value;
            HeapifyUp(parent);
        }
    }
    private static int ParentIndex(int i)
    {
        return (i - 1) / 2;
    }
    private static int LeftChildIndex(int i)
    {
        return 2 * i + 1;
    }
    private static int RightChildIndex(int i)
    {
        return 2 * i + 2;
    } 
}