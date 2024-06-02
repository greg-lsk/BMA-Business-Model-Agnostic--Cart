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

internal interface ICondition
{
    public bool AppliesTo<TSubject>(TSubject subject);
}

internal class ConditionPipe<TSubject> : ICondition
{
    private readonly List<Predicate<TSubject>> _conditions = [];

    internal void AddCondition(Predicate<TSubject> condition)
    {
        _conditions.Add(condition);
    }

    public bool AppliesTo<TTraverver>(TTraverver subject)
    {   
        //Valid Type logic
        if(typeof(TSubject) != typeof(TTraverver))
            return false;
        //Valid Type logic

        return true;
    }
}

internal class ConditionGroup : ICondition
{
    private readonly Dictionary<Type, ICondition> _groupedConditions = [];

    public bool AppliesTo<TSubject>(TSubject subject)
    {
        if(_groupedConditions.ContainsKey(typeof(TSubject)))
        {
            return _groupedConditions[typeof(TSubject)].AppliesTo(subject);
        }

        return false;
    }
}