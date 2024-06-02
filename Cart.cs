using System.Collections.ObjectModel;

namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate int QuantityUpdateDelegate(int inCartQuantity);

public class Cart<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly StockedCollection<TProduct> _items 
    = new(equalityDelegate);

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    public void Add(TProduct product, int quantity = 1) => 
    _items.Add
    (
        product, 
        quantity, 
        () => _items.Contains(product), 
        () => _items.UpdateQuantity(product, inCart => inCart + quantity) 
    );
 
    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateDelegate) 
        => _items.UpdateQuantity(product, updateDelegate);
    
    public ReadOnlyCollection<(TProduct Product, int Quantity)> Content() => _items.AsReadonly();

    //Pricing
}