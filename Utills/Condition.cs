namespace Cart;


public class ParameterContext<TSubject>(TSubject subject)
{
    internal TSubject Subject = subject;
}


internal delegate bool ConditionDelegate<TSubject>(TSubject subject, ParameterContext<TSubject> context);

public interface ICondition
{
    public bool AppliesTo<TSubject>(TSubject subject, ParameterContext<TSubject>? context = null);
}

internal class ConditionPipe<TSubject> : ICondition
{
    private readonly List<ConditionDelegate<TSubject>> _conditions = [];

    internal void AddCondition(ConditionDelegate<TSubject> condition)
    {
        _conditions.Add(condition);
    }

    public bool AppliesTo<TTraverver>(TTraverver subject, ParameterContext<TTraverver>? context = null)
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

    public bool AppliesTo<TSubject>(TSubject subject, ParameterContext<TSubject>? context = null)
    {
        if(_groupedConditions.ContainsKey(typeof(TSubject)))
        {
            return _groupedConditions[typeof(TSubject)].AppliesTo(subject, context);
        }

        return false;
    }
}