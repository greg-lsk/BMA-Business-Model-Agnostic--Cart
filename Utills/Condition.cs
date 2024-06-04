namespace Cart;

public class ParameterProvider
{
    private readonly Dictionary<int, object> _params = [];

    public object this[int index]
    {
        get => _params[index];
        set => _params[index] = value;
    }    
}

internal delegate bool ConditionDelegate<TSubject>(TSubject subject, ParameterProvider provider);

public interface ICondition
{
    public bool AppliesTo<TSubject>(TSubject subject, ParameterProvider? provider = null);
}

internal class ConditionPipe<TSubject> : ICondition
{
    private readonly List<ConditionDelegate<TSubject>> _conditions = [];

    internal void AddCondition(ConditionDelegate<TSubject> condition)
    {
        _conditions.Add(condition);
    }

    public bool AppliesTo<TTraverver>(TTraverver subject, ParameterProvider? provider = null)
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

    public bool AppliesTo<TSubject>(TSubject subject, ParameterProvider? provider = null)
    {
        if(_groupedConditions.ContainsKey(typeof(TSubject)))
        {
            return _groupedConditions[typeof(TSubject)].AppliesTo(subject, provider);
        }

        return false;
    }
}