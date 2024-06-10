﻿namespace Utils;

internal delegate TReturn IterationFunction<TEntry, TReturn>(IEnumerable<TEntry> sequence,
                                                             EntryActionTracked<TEntry, TReturn> entryAction);
internal delegate void IterationAction<TEntry>(IEnumerable<TEntry> sequence,
                                               EntryAction<TEntry> entryAction);

internal delegate void EntryAction<TEntry>(Iterator<TEntry> iterator);
internal delegate void EntryActionTracked<TEntry, TReturn>(Tracker<TReturn> tracker,
                                                           Iterator<TEntry> iterator);


internal ref struct Iterator<TEntry>(List<TEntry> list)
{
    private readonly List<TEntry> _list = list;
    private int _currentIndex = 0;

    internal readonly TEntry Current
    { 
        get => _list[_currentIndex];
        set => _list[_currentIndex] = value;
    }

    internal bool IsFrozen { get; private set; } = false;
     
    internal void Break() => IsFrozen = true;
    internal void Next() => _currentIndex++;  
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
            entryAction(iterator);

            if(iterator.IsFrozen) break;
            else iterator.Next();
        }        
    }

    internal TReturn? Run<TReturn>(EntryActionTracked<TEntry, TReturn> entryActionTracked)
    {
        var list = _sequence.ToList();
        var iterator = new Iterator<TEntry>(list);
        var returnTracker = new Tracker<TReturn>();

        for(int i = 0; i < _sequence.Count(); ++i)
        {
            entryActionTracked(returnTracker, iterator);

            if(iterator.IsFrozen) break;
            else iterator.Next();                
        }

        return returnTracker.Captured;               
    }
}