using System;

/// <summary>
/// Source: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
/// </summary>
public class Heap<T> where T : IHeapItem<T>
{
    public int Count => count;
    
    private readonly T[] items;
    private int count;

    public Heap(int length)
    {
        if (length <= 0)
            throw new Exception();

        items = new T[length];
    }

    public void Add(T item)
    {
        item.HeapIndex = count;
        items[count] = item;
        SortUp(item);
        count++;
    }

    public T RemoveFirst()
    {
        T first = items[0];
        count--;

        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return first;
    }

    public void UpdateItem(T item) => SortUp(item);
    public bool Contains(T item) => Equals(items[item.HeapIndex], item);

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex;

            if (childIndexLeft < count)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < count)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int temp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = temp;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}