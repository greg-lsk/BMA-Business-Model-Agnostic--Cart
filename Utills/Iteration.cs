namespace Utils;

internal delegate TReturn IterationFunction<TEntry, TReturn>(EntryFunction<TEntry, TReturn> entryAction);
internal delegate void IterationAction<TEntry>(EntryAction<TEntry> entryAction);


internal delegate void EntryVoid<TEntry>(Iterator<TEntry> current);
internal delegate Operation EntryAction<TEntry>(Iterator<TEntry> current);
internal delegate (TReturn ReturnType, Operation OperationCommand) EntryFunction<TEntry, TReturn>(Iterator<TEntry> current);


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

internal readonly ref struct Tracker<TSubject>(in TSubject? subject = default)
{
    private readonly ref readonly TSubject? _subject = ref subject;
    internal readonly bool IsActive => _subject is not null;
    
    internal static Tracker<TSubject> Start(in TSubject subject) => new(in subject); 
}

internal readonly struct Iteration
{
    internal static TReturn? On<TEntry, TReturn>(IEnumerable<TEntry> sequence,
                                                 EntryFunction<TEntry, TReturn> entryAction)
    {
        TReturn? returnType = default;
        var list = sequence.ToList();

        for (int i = 0; i < sequence.Count(); ++i)
        {
            Operation operationCommand;
            (returnType, operationCommand) = entryAction(new(list, i));

            if (operationCommand is Operation.Break) break;
        }

        return returnType;
    }

    internal static void On<TEntry>(IEnumerable<TEntry> sequence,
                                    EntryAction<TEntry> entryAction)
    {
        var list = sequence.ToList();

        for (int i = 0; i < sequence.Count(); ++i)
        {
            Operation operationCommand = entryAction(new(list, i));

            if (operationCommand is Operation.Break) break;
        }
    }

    internal static void On<TEntry>(IEnumerable<TEntry> sequence,
                                    EntryVoid<TEntry> entryAction)
    {
        var list = sequence.ToList();

        for (int i = 0; i < sequence.Count(); ++i) entryAction(new(list, i)); 
    }            
}

internal enum Operation
{
    Break,
    Continue
}