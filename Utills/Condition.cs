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

internal static class ConditionPipe01
{
    internal static Condition Check<TSubject>(TSubject? subject, Predicate<TSubject>? predicate)
    {
        return new( predicate(subject) );
    }
}


internal interface ITraversable
{
    public bool TraverseWith<TSubject>(TSubject subject);
}

internal class ConditionPipe02<TSubject> : ITraversable
{
    private readonly List<Predicate<TSubject>> _conditions = [];

    internal void AddCondition(Predicate<TSubject> condition)
    {
        _conditions.Add(condition);
    }

    public bool TraverseWith<TTraverver>(TTraverver subject)
    {   
        //Valid Type logic
        if(typeof(TSubject) != typeof(TTraverver))
            return false;
        //Valid Type logic

        return true;
    }
}

internal class ConditionGroup
{
    private readonly Dictionary<Type, ITraversable> _groupedConditions = [];

    public bool TraverseWith<TSubject>(TSubject subject)
    {
        if(_groupedConditions.ContainsKey(typeof(TSubject)))
        {
            return _groupedConditions[typeof(TSubject)].TraverseWith(subject);
        }

        return false;
    }
}