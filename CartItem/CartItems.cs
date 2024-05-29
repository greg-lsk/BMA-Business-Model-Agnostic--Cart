namespace Cart;

internal class CartItems<TProduct>
{
    private readonly List<(TProduct Product, int Quantity)> _items = [];

    internal int CountDistinct => _items.Count;
    internal int CountTotal
    {
        get 
        {
            int total = 0;
 
            for(int i=0; i < _items.Count; ++i)
                total += _items[i].Quantity; 
            
            return total;  
        }
    }

    internal (TProduct Product, int Quantity) this[int index] => _items[index];

    internal void Add(TProduct product, int quantity)
    {
        var info = TryGetQuantity(product);

        if( info.index == -1 )
        {
            _items.Add( (product, quantity) );
            return;
        }

        _items[info.index] = (product, quantity + info.quantity);
    }

    internal void RemoveAt(int index) => _items.RemoveAt(index);


    private (int index , int quantity) TryGetQuantity(TProduct product)
    {
        int foundAt = -1;
        int quantity = 0;

        for(int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];

            if ( !AreTheSame(Product, product) ) continue;

            foundAt = i;
            quantity = Quantity;
            break;
        }

        return (index: foundAt, quantity: quantity);
    }

    private static bool AreTheSame(TProduct product1, TProduct product2) => product1.Equals(product2);
}