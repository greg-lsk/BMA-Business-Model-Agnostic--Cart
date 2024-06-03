namespace Cart;


internal class ParameterContext<TSubject>
{
    internal TSubject Subject;
}

public interface ICondition
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