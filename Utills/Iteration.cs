﻿namespace Utils;

internal delegate TReturn IterationFunction<TEntry, TReturn>(EntryFunction<TEntry, TReturn> entryAction);
internal delegate void IterationAction<TEntry>(EntryAction<TEntry> entryAction);


internal delegate void EntryAction<TEntry>(Iterator<TEntry> current);
internal delegate void EntryFunction<TEntry, TReturn>(Tracker<TReturn> tracker, Iterator<TEntry> current);


internal ref struct Iterator<TEntry>(List<TEntry> list)
{
    private readonly List<TEntry> _list = list;
    private int _currentIndex = -1;

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
    internal static TReturn? On<TEntry, TReturn>(IEnumerable<TEntry> sequence,
                                                 EntryFunction<TEntry, TReturn> entryFunction)
    => Loop(sequence, entryFunction);
    
    private static TReturn? Loop<TEntry, TReturn>(IEnumerable<TEntry> sequence,
                                                  EntryFunction<TEntry, TReturn> entryFunction)
    {
        var list = sequence.ToList();
        var iterator = new Iterator<TEntry>(list);
        var returnTracker = new Tracker<TReturn>();

        for(int i = 0; i < sequence.Count(); ++i)
        {
            entryFunction(returnTracker, iterator);

            if(iterator.IsFrozen) break;
            else iterator.Next();                
        }

        return returnTracker.Captured;               
    } 


    internal static void On<TEntry>(IEnumerable<TEntry> sequence, EntryAction<TEntry> entryAction) 
    => Loop(sequence, entryAction); 

    private static void Loop<TEntry>(IEnumerable<TEntry> sequence,
                                     EntryAction<TEntry> entryAction)
    {
        var list = sequence.ToList();
        var iterator = new Iterator<TEntry>(list);

        for(int i = 0; i < sequence.Count(); ++i)
        {
            entryAction(iterator);

            if(iterator.IsFrozen) break;
            else iterator.Next();
        }        
    }            
}