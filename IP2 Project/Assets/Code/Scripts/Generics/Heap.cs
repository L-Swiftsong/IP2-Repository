using System;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] _items;

    private int _currentItemCount;
    public int Count => _currentItemCount;


    public Heap(int maxHeapSize) => _items = new T[maxHeapSize];


    public void Add(T item)
    {
        // Add the item to the heap.
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;

        // Sort the item up the Heap.
        SortUp(item);
        _currentItemCount++;
    }
    public T RemoveFirst()
    {
        // Cache the first item.
        T firstItem = _items[0];
        _currentItemCount--;

        // Take the item at the end of the heap & put it into the first place.
        _items[0] = _items[_currentItemCount];
        _items[0].HeapIndex = 0;

        // Sort the heap.
        SortDown(_items[0]);

        // Return the first index.
        return firstItem;
    }
    public void Clear() => _currentItemCount = 0;

    public void UpdateItem(T item) => SortUp(item);


    public bool Contains(T item)
    {
        // If the item's index is within the current heap size, return true if the elements are equal.
        if (item.HeapIndex < _currentItemCount)
            return Equals(_items[item.HeapIndex], item);
        // Return false if the heapIndex falls outside of the current heap size.
        else
            return false;
    }



    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = _items[parentIndex];

            // Compare the current item to its parent.
            if (item.CompareTo(parentItem) > 0)
            {
                // If the item has a larger priority, then swap them.
                SwapItems(item, parentItem);
            }
            // Stop looping if the item no longer has a larger priority than its parent, then stop looping.
            else
                break;

            // Update the parent index.
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    private void SortDown(T item)
    {
        // Loop until sorted.
        while (true)
        {
            int childLeftIndex = (item.HeapIndex) * 2 + 1;
            int childRightIndex = (item.HeapIndex) * 2 + 2;
            int swapIndex = 0;

            // If the left child exists, set it as the target.
            if (childLeftIndex < _currentItemCount)
            {
                swapIndex = childLeftIndex;

                // If the right child exists and has a higher priority, then set that child as the target.
                if (childRightIndex < _currentItemCount && _items[childRightIndex].CompareTo(_items[childLeftIndex]) > 0)
                    swapIndex = childRightIndex;


                // Check if the parent has a lower priority with its highest priority child, then swap them.
                if (item.CompareTo(_items[swapIndex]) < 0)
                {
                    SwapItems(item, _items[swapIndex]);
                }
                // If the parent has a larger priority than its highest priority child, then it is in the correct position and we can stop looping.
                else
                    return;
            }
            // If the parent has no children, then it is in the correct position and we can stop looping.
            else
                return;
        }
    }



    private void SwapItems(T item1, T item2)
    {
        // Swap the position of the elements in the array.
        _items[item1.HeapIndex] = item2;
        _items[item2.HeapIndex] = item1;

        // Swap the HeapIndexes of the items.
        int item1Index = item1.HeapIndex;
        item1.HeapIndex = item2.HeapIndex;
        item2.HeapIndex = item1Index;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}