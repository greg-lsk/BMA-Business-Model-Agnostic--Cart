namespace Utils;

internal delegate void Iteration<TEntry>(EntryAction<TEntry> entryAction);
internal delegate void EntryAction<TEntry>(Iterator<TEntry> current);

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

internal readonly struct Iteration
{
    internal static Iteration<TEntry> For<TEntry>(IEnumerable<TEntry> collection) =>
    (EntryAction<TEntry> entryAction) =>
    {
        for(int i = 0; i < collection.Count(); ++i)
        {
            entryAction(new(collection.ToList() , i));
        }
    };
}