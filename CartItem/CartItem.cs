namespace Cart;

internal readonly struct CartItem<TProduct>(TProduct product, int quantity = 0)
{
    internal readonly int Quantity = quantity;
    internal readonly TProduct Product = product;


    public static bool operator ==(CartItem<TProduct> item1, CartItem<TProduct> item2) 
        => item1.Product.Equals(item2.Product);
        
    public static bool operator !=(CartItem<TProduct> item1, CartItem<TProduct> item2)
        => !(item1 == item2);        

    public override int GetHashCode() => Product.GetHashCode();
}