namespace Cart;

internal class CartItems<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly List<(TProduct Product, int Quantity)> _items = [];
    private readonly EqualityDelegate<TProduct> _equals = equalityDelegate;

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
        for(int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];

            if(_equals(Product, product))
            {
                _items[i] = (Product, Quantity + quantity);
                return;
            }
        }

        _items.Add((product, quantity));
    }

    internal void Delete(TProduct product)
    {
        for(int i = 0; i < _items.Count; ++i)
        {
            if( _equals(_items[i].Product, product) ) 
            { 
                _items.RemoveAt(i); 
                return; 
            }
        }
    }
   
    internal void UpdateQuantity(TProduct product, QuantityUpdateDelegate update)
    {
        for(int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if( _equals(Product, product) )
            {
                _items[i] = (Product, update(Quantity));
                return;
            }
        } 
    }

    internal int CountOf(TProduct product)
    {
        for(int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if( _equals(Product, product) ) return Quantity;
        }

        return 0;
    }
}