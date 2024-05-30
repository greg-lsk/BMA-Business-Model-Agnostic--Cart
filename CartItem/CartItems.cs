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

        if( !ProductFound(info.Index) )
        {
            _items.Add( (product, quantity) );
            return;
        }

        _items[info.Index] = (product, quantity + info.Quantity);
    }

    internal void Delete(TProduct product)
    {
        int atIndex = TryGetIndexOf(product);

        if( !ProductFound(atIndex) ) return;

        _items.RemoveAt(atIndex);
    }

    internal void UpdateQuantity(TProduct product, int quantity)
    {
        int atIndex = TryGetIndexOf(product);

        if( !ProductFound(atIndex) ) return;

        _items[atIndex] = (product, quantity); 
    }

    private (int Index , int Quantity) TryGetQuantity(TProduct product)
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

        return (Index: foundAt, Quantity: quantity);
    }

    private int TryGetIndexOf(TProduct product)
    {
        int foundAt = -1;

        for(int i = 0; i < _items.Count; ++i)
        {
            if ( AreTheSame(_items[i].Product, product) ) return i;
        }

        return foundAt;
    }

    private static bool ProductFound(int index) => index >= 0;
    private static bool AreTheSame(TProduct product1, TProduct product2) => product1.Equals(product2);
}