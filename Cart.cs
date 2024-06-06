using System.Collections.ObjectModel;

namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate int QuantityUpdateDelegate(int inCartQuantity);

public class Cart<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly StockedCollection<TProduct> _items = new(equalityDelegate);
    private EqualityDelegate<TProduct> _equals = equalityDelegate;

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    public bool Contains(TProduct product) => _items.Contains(product, _equals); 

    public void Add(TProduct product, int quantity = 1) =>    
    _items.Iteration(i =>
    {
        var (Item, Quantity) = i.Current;

        if(_equals(Item, product))
        {
            Quantity += quantity;
            return;
        }

        //This should be outside the Iteration!!!
        _items.NewEntry(product, quantity);
    });
    

    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateQuantity) =>
    _items.Iteration(i =>
    {
        var (Item, Quantity) = i.Current;
        if(_equals(Item, product)) 
        {
            i.Current = new(product, updateQuantity(Quantity)); 
            return;
        }
    }); 
        
    
    public ReadOnlyCollection<(TProduct Product, int Quantity)> Content() => _items.AsReadonly();

    //Pricing
}