namespace Cart;

internal delegate void Iteration<TEntry>(EntryAction<TEntry> entryAction);
internal delegate void EntryAction<TEntry>(Iterator<(TEntry Item, int Quantity)> current);

internal readonly struct Iterator<TEntry>(List<TEntry> list, int index)
{
    private readonly List<TEntry> _list = list;
    private readonly int _index = index;

    internal TEntry Current
    {
        get => _list[_index];
        set => _list[_index] = value;
    }
}