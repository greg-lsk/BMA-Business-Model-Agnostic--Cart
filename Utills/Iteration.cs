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
    internal void Reset() {_currentIndex = _indexerStart; _isBroken = false;} 
}

internal readonly struct Iteration
{
    internal static ActionProvider<TEntry> On<TEntry>(IEnumerable<TEntry> sequence) => new(sequence);
}

internal readonly struct ActionProvider<TEntry>(IEnumerable<TEntry> sequence)
{
    private readonly IEnumerable<TEntry> _sequence = sequence;

    internal void Run(ActionViaIterator<TEntry> action) 
    => IterationCore.Loop(_sequence, action);

    internal TReturn? Run<TReturn>(FunctionViaIterator<TEntry, TReturn> function) 
    => IterationCore.Loop(_sequence, function);

    internal void Run(EntryAction<TEntry> action) 
    => IterationCore.Loop(_sequence, (ref Iterator<TEntry> i) => action(i.Current));


    internal ConditionalProvider<TEntry> When(Predicate<TEntry> condition) => new(_sequence, condition);
    internal MappingProvider<TEntry> Apply(Predicate<TEntry> condition) => new(_sequence, condition);          
}

internal readonly struct ConditionalProvider<TEntry>(IEnumerable<TEntry> sequence, Predicate<TEntry> condition)
{
    private readonly IEnumerable<TEntry> _sequence = sequence;
    private readonly Predicate<TEntry> _condition = condition;

    internal TReturn? Run<TReturn>(EntryFunction<TEntry, TReturn> function) => default;        
}

internal readonly struct MappingProvider<TEntry>(IEnumerable<TEntry> sequence, Predicate<TEntry> condition)
{
    private readonly IEnumerable<TEntry> _sequence = sequence;
    private readonly Predicate<TEntry> _condition = condition;

    internal TReturn? Map<TReturn>(
        Func<bool, PostEvaluationFunction<TEntry, TReturn>> mapper,
        Func<TReturn>? none = default) => default;    
}

internal static class IterationCore
{    
    internal static void Loop<TEntry>(IEnumerable<TEntry> sequence,
                                      ActionViaIterator<TEntry> action)
    {
        var iterator = new Iterator<TEntry>(sequence);

        do{
            action(ref iterator);
            iterator.Move();
        }while(iterator.CanIncrement);
    }

    internal static TReturn? Loop<TEntry, TReturn>(IEnumerable<TEntry> sequence,
                                                   FunctionViaIterator<TEntry, TReturn> function)
    {
        var iterator = new Iterator<TEntry>(sequence);
        
        TReturn? returnValue;

        do{
            returnValue = function(ref iterator);
            iterator.Move();
        }while(iterator.CanIncrement);

        return returnValue;
    }
}

internal static class ConditionCore
{
    internal static void ActWhen<TEntry>(IEnumerable<TEntry> sequence,
                                         Predicate<TEntry> condition,
                                         EntryAction<TEntry> action) 
    => IterationCore.Loop(
        sequence, 
        (ref Iterator<TEntry> i) => 
        {
            if(condition(i.Current))
            {
                action(i.Current);
                i.Break();
            }            
        });
 
    
    internal static TReturn? ActWhen<TEntry, TReturn>(IEnumerable<TEntry> sequence,
                                                      Predicate<TEntry> condition,
                                                      EntryFunction<TEntry, TReturn> function) 
    => IterationCore.Loop(
        sequence, 
        (ref Iterator<TEntry> i) => 
        {
            TReturn? returnValue = default;

            if(condition(i.Current))
            {
                returnValue = function(i.Current);
                i.Break();
            }

            return returnValue;            
        });
}

internal readonly struct PostEvaluationFunction<TEntry, TReturn>
{
    internal Delegate? Delegate { get; } = null;

    internal PostEvaluationFunction(TReturn? returnValue) {Delegate = () => returnValue;}

    internal PostEvaluationFunction(Func<TReturn> function) {Delegate = function;}
    internal PostEvaluationFunction(Func<TEntry, TReturn> function) {Delegate = function;}         
}