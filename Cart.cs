using System.Collections.ObjectModel;

namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate void QuantityUpdateDelegate(out int inCartQuantity);

public class Cart<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly StockedCollection<TProduct> _items = new(equalityDelegate);
    private EqualityDelegate<TProduct> _equalityDelegate = equalityDelegate;

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    public bool Contains(TProduct product) => _items.Contains(product, _equalityDelegate); 

    public void Add(TProduct product, int quantity = 1) =>    
    _items.Iteration((TProduct p, ref int q) =>
    {
        if(_equalityDelegate(p, product))
        {
            q += quantity;
            return;
        }
        _items.NewEntry(product, quantity);
    });
    

    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateDelegate) =>
    _items.Iteration((TProduct p, ref int q) =>
    {
        if(_equalityDelegate(p, product)) 
        {
            updateDelegate(out q); 
            return;
        }
    }); 
        
    
    public ReadOnlyCollection<(TProduct Product, int Quantity)> Content() => _items.AsReadonly();

    //Pricing
}