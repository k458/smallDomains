namespace Essentials;

public class StableIndexList<T>
{
    private readonly List<T?> items = [];
    private readonly Stack<int> freedIndexes = [];
    private int nextIndex;

    public IReadOnlyList<T?> Items => items;
    public IReadOnlyCollection<int> FreedIndexes => freedIndexes;
    public int NextIndex => nextIndex;

    public T? this[int index]
    {
        get
        {
            if (index < 0 || index >= items.Count)
            {
                return default;
            }

            return items[index];
        }
    }

    public int Add(T item)
    {
        int index = GetNextAvailableIndex();

        while (items.Count <= index)
        {
            items.Add(default);
        }

        items[index] = item;
        return index;
    }

    public bool TryGet(int index, out T? item)
    {
        item = default;

        if (index < 0 || index >= items.Count)
        {
            return false;
        }

        item = items[index];
        return item is not null;
    }

    public bool TryRemoveAt(int index)
    {
        if (index < 0 || index >= items.Count || items[index] is null)
        {
            return false;
        }

        items[index] = default;
        freedIndexes.Push(index);
        return true;
    }

    public void Clear()
    {
        freedIndexes.Clear();
        nextIndex = 0;
    }

    public void Nullify()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = default;
        }

        Clear();
    }

    private int GetNextAvailableIndex()
    {
        if (freedIndexes.Count > 0)
        {
            return freedIndexes.Pop();
        }

        return nextIndex++;
    }
}
