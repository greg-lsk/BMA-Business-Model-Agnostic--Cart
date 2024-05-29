namespace Cart;

internal readonly struct CartItem<TProduct>(TProduct product, int quantity = 0)
{
    internal readonly int Quantity = quantity;
    internal readonly TProduct Product = product;
}