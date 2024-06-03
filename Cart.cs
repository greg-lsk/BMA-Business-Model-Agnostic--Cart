using System.Collections.ObjectModel;

namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate int QuantityUpdateDelegate(int inCartQuantity);

public class Cart<TProduct>
{
    private readonly StockedCollection<TProduct> _items;
    private EqualityDelegate<TProduct> _equalityDelegate;
    private ICondition _equality;

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    public Cart(EqualityDelegate<TProduct> equalityDelegate,
                ICondition? equalityCondition = null)
    {
        _equalityDelegate = equalityDelegate;

        _equality = equalityCondition is not null 
            ? equalityCondition
            : new ConditionPipe<TProduct>();

        (_equality as ConditionPipe<TProduct>).AddCondition((p ,c) => _equalityDelegate(p, c.Subject));

        _items = new(_equality);
    }

    public void Add(TProduct product, int quantity = 1) => 
    _items.Add
    (
        product, 
        quantity, 
        () => _items.Contains(product, _equality), 
        () => _items.UpdateQuantity(product, inCart => inCart + quantity) 
    );
 
    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateDelegate) 
        => _items.UpdateQuantity(product, updateDelegate);
    
    public ReadOnlyCollection<(TProduct Product, int Quantity)> Content() => _items.AsReadonly();

    //Pricing
}