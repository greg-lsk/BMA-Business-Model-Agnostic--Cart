namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate int QuantityUpdateDelegate(int inCartQuantity);

public class Cart<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly CartItems<TProduct> _items = new(equalityDelegate);

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    public void Add(TProduct product, int quantity = 1) => _items.Add(product, quantity);
    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateDelegate) 
        => _items.UpdateQuantity(product, updateDelegate);
    
    //Get Items
    //Pricing
}