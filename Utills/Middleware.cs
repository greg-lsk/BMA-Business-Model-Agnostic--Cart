namespace Cart;


internal readonly struct AddMiddlewareBuilder
{
    internal static AddMiddleware Initialize() => new();
}

internal readonly struct AddMiddleware(bool state = true)
{
    private readonly bool _state = state;

    internal AddMiddleware When(Func<bool> cond)
    {
        if(!_state) return new(false);

        if( cond.Invoke() ) return new(true);
        else return new(false);
    }

    internal AddMiddleware Do(Action action)
    {
        if(!_state) return new(false);

        action.Invoke();
        return this;
    }
}