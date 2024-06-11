namespace Utils;

internal delegate void EntryAction<TEntry>(ref Iterator<TEntry> iterator);
internal delegate TReturn? EntryFunction<TEntry, TReturn>(ref Iterator<TEntry> iterator);


internal ref struct Iterator<TEntry>(IEnumerable<TEntry> sequence)
{
    private readonly IEnumerable<TEntry> _sequence = sequence;
    private int _currentIndex = 0;
    private bool _isBroken = false;

    internal readonly int CurrentIndex => _currentIndex;
    internal readonly TEntry Current => _sequence.ElementAt(_currentIndex);

    internal readonly bool Finished => _currentIndex > _sequence.Count();
    internal readonly bool IsBroken => _isBroken;

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

    internal void Run(EntryAction<TEntry> entryAction)
    {
        var list = _sequence.ToList();
        var iterator = new Iterator<TEntry>(list);

        for(int i = 0; i < _sequence.Count(); ++i)
        {
            entryAction(ref iterator);

            if(iterator.IsBroken) break;
            else iterator.Move();
        }        
    }

    internal TReturn? Run<TReturn>(EntryFunction<TEntry, TReturn> entryActionTracked)
    {
        var list = _sequence.ToList();
        var iterator = new Iterator<TEntry>(list);

        TReturn? returnValue = default;
        for(int i = 0; i < _sequence.Count(); ++i)
        {
            returnValue = entryActionTracked(ref iterator);
            if(iterator.IsBroken) return returnValue; 
            else iterator.Move();                
        }
        return returnValue;               
    }
}