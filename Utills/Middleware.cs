namespace Cart;


internal readonly struct AddMiddlewareBuilder
{
    internal static AddMiddleware Initialize() => new();
}

internal readonly struct AddMiddleware(bool state = true)
{
    private readonly bool _state = state;

    internal AddMiddleware When<TSubject>(TSubject subject, Func<TSubject, bool> cond)
    {
        if(!_state) return new(false);

        if( !cond(subject) ) return new(true);
        else return new(false);
    }

    internal AddMiddleware Do<TSubject>(TSubject subject, Action<TSubject> action)
    {
        if(!_state) return new(false);

        action(subject);
        return this;
    }
}