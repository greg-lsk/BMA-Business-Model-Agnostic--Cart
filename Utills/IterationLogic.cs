namespace Cart;

internal delegate bool CurrentCondition<TItem>(TItem product, int quantity = 0);
internal delegate void OnHit();
internal delegate void OnMiss();

internal readonly struct IterationLogic<TItem>
{
    internal readonly CurrentCondition<TItem> Check;

    public IterationLogic(CurrentCondition<TItem> check,
                          OnHit? onHit = null,
                          OnMiss? onMiss = null) : this()
    {
        Check = check;
        OnHit = onHit;
        OnMiss = onMiss;
    }

    internal readonly OnHit? OnHit;
    internal readonly OnMiss? OnMiss; 
}