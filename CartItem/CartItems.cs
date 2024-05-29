namespace Cart;

internal class CartItems<TProduct>(List<CartItem<TProduct>> items)
{
    private readonly List<CartItem<TProduct>> _items = items;


    internal void Add(TProduct product, int quantity)
    {
        var info = TryGetQuantity(product);

        if( info.index == -1 )
        {
            _items.Add( new CartItem<TProduct>(product, quantity));
            return;
        }

        _items[info.index] = new CartItem<TProduct>(product, quantity + info.quantity);
    }


    private (int index , int quantity) TryGetQuantity(TProduct product)
    {
        int foundAt = -1;
        int quantity = 0;

        for(int i = 0; i < _items.Count; ++i)
        {
            var item = _items[i];

            if ( !AreTheSame(item.Product, product) ) continue;

            foundAt = i;
            quantity = item.Quantity;
            break;
        }

        return (index: foundAt, quantity: quantity);
    }

    private static bool AreTheSame(TProduct product1, TProduct product2) => product1.Equals(product2);
}