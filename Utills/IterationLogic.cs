namespace Cart;

internal delegate bool CurrentCondition<TItem>(TItem? product = default, int? quantity = 0);
internal delegate void OnHit<TItem>(TItem? product = default, int? quantity = 0);
internal delegate void OnMiss<TItem>(TItem? product = default, int? quantity = 0);

internal readonly struct IterationLogic<TItem>
{
    internal readonly CurrentCondition<TItem> Check;

    public IterationLogic(CurrentCondition<TItem> check,
                          OnHit<TItem>? onHit = null,
                          OnMiss<TItem>? onMiss = null) : this()
    {
        Check = check;
        OnHit = onHit;
        OnMiss = onMiss;
    }

    internal readonly OnHit<TItem>? OnHit;
    internal readonly OnMiss<TItem>? OnMiss; 
}