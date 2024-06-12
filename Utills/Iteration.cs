namespace Utils;

internal delegate void ActionViaIterator<TEntry>(ref Iterator<TEntry> iterator);
internal delegate TReturn? FunctionViaIterator<TEntry, TReturn>(ref Iterator<TEntry> iterator);

internal delegate void EntryAction<TEntry>(TEntry entry);
internal delegate TReturn? EntryFunction<TEntry, TReturn>(TEntry entry);

internal ref struct Iterator<TEntry>(IEnumerable<TEntry> sequence)
{
    private const int _indexerStart = 0;
    private const int _indexOffset = 1 - _indexerStart; 

    private readonly IEnumerable<TEntry> _sequence = sequence;
    private int _currentIndex = _indexerStart;
    private bool _isBroken = false;

    internal readonly int CurrentIndex => _currentIndex;
    internal readonly TEntry Current => _sequence.ElementAt(_currentIndex);

    internal readonly bool Finished => _currentIndex + _indexOffset > _sequence.Count();
    internal readonly bool IsBroken => _isBroken;
    internal readonly bool CanIncrement => !IsBroken && !Finished; 

    internal void Break() => _isBroken = true;
    internal void Move(int step = 1) => _currentIndex+=step;
    internal void Reset() {_currentIndex = 0; _isBroken = false;} 
}

internal ref struct Tracker<TSubject>
{
    private ref TSubject? _subject;

    public Tracker(ref TSubject? subject) => _subject = ref subject;

    internal readonly TSubject? Captured => _subject;

    internal void Capture(in TSubject? subject) => _subject = subject;
     
}

internal readonly struct Iteration
{
    internal static ActionProvider<TEntry> On<TEntry>(IEnumerable<TEntry> sequence) => new(sequence);
}

internal readonly struct ActionProvider<TEntry>(IEnumerable<TEntry> sequence)
{
    private readonly IEnumerable<TEntry> _sequence = sequence;

    internal void Run(ActionViaIterator<TEntry> action) => Loop(action);
    internal TReturn? Run<TReturn>(FunctionViaIterator<TEntry, TReturn> function) => Loop(function);

    internal void Run(EntryAction<TEntry> action) => Loop((ref Iterator<TEntry> i) => action(i.Current));

    internal void ActWhen(Predicate<TEntry> condition, Action<TEntry> action) => 
    Loop((ref Iterator<TEntry> i) => 
    {
        if(condition(i.Current))
        {
            action(i.Current);
            i.Break();
        }
    });

    internal TReturn? ActWhen<TReturn>(Predicate<TEntry> condition, EntryFunction<TEntry, TReturn> function) => 
    Loop((ref Iterator<TEntry> i) => 
    {
        TReturn? returnValue = default;
        if(condition(i.Current))
        {
            returnValue = function(i.Current);
            i.Break();
        }
        return returnValue;
    });     
    
    private void Loop(ActionViaIterator<TEntry> action)
    {
        var iterator = new Iterator<TEntry>(_sequence);

        do{
            action(ref iterator);
            iterator.Move();
        }while(iterator.CanIncrement);
    }

    private TReturn? Loop<TReturn>(FunctionViaIterator<TEntry, TReturn> function)
    {
        var iterator = new Iterator<TEntry>(_sequence);
        
        TReturn? returnValue;

        do{
            returnValue = function(ref iterator);
            iterator.Move();
        }while(iterator.CanIncrement);

        return returnValue;
    }    
}