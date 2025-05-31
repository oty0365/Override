using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class PriorityQueue<T>
{
    private List<(T item, int priority)> heap = new();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("Queue is empty");

        var root = heap[0].item;
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return root;
    }

    public T RandomDequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        var minimum = heap.Min(item => item.priority);

        var randomList = new List<(T item, int priority)>();
        foreach (var i in heap)
        {
            if (i.priority == minimum)
            {
                randomList.Add(i);
            }
        }
        foreach(var i in randomList)
        {
            UnityEngine.Debug.Log(i);
        }
        int randomIndex = UnityEngine.Random.Range(0, randomList.Count);
        var selected = randomList[randomIndex];

        heap.Remove(selected);

        return selected.item;
    }


    public void Clear()
    {
        heap.Clear();
    }

    private void HeapifyUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority >= heap[parent].priority) break;
            (heap[i], heap[parent]) = (heap[parent], heap[i]);
            i = parent;
        }
    }

    private void HeapifyDown(int i)
    {
        while (true)
        {
            int left = 2 * i + 1, right = 2 * i + 2, smallest = i;
            if (left < heap.Count && heap[left].priority < heap[smallest].priority) smallest = left;
            if (right < heap.Count && heap[right].priority < heap[smallest].priority) smallest = right;
            if (smallest == i) break;
            (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
            i = smallest;
        }
    }
}
