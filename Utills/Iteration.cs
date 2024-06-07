namespace Utils;

internal delegate TReturn Iteration<TEntry, TReturn>(EntryAction<TEntry, TReturn> entryAction);
internal delegate (Operation OperationCommand, TReturn ReturnType) EntryAction<TEntry, TReturn>(Iterator<TEntry> current);

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
    internal static Iteration<TEntry, TReturn> For<TEntry, TReturn>(IEnumerable<TEntry> collection) =>
    (entryAction) =>
    {
        TReturn returnType = default;
        Operation operationCommand = Operation.Continue;

        for(int i = 0; i < collection.Count(); ++i)
        {
            (operationCommand, returnType) = entryAction(new(collection.ToList() , i));

            if(operationCommand is Operation.Seize) break;
        }

        return returnType;
    };
}

internal enum Operation
{
    Seize,
    Continue,
    Finished
}