namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);

public class Cart<TProduct>(EqualityDelegate<TProduct> equals)
{
    private readonly CartItems<TProduct> _items = new(equals);

    //Add Product
    //Delete Product

    //Update Quantity of Product(increase AND reduce)

    //Get Items

    //Pricing
}