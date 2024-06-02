namespace Cart;

internal readonly struct Condition(bool result = true)
{
    private readonly bool _result = result;
    
    internal Condition Check<TSubject>(TSubject? subject, Predicate<TSubject>? predicate)
    {
        return new( predicate(subject) );
    }

    internal bool Result() => _result;
}

internal static class ConditionPipe
{
    internal static Condition Check<TSubject>(TSubject? subject, Predicate<TSubject>? predicate)
    {
        return new( predicate(subject) );
    }
}