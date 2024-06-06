using System.Collections.ObjectModel;

namespace Cart;

internal delegate void NewEntry<TEntity>(TEntity ofEntity, int quantity);  
internal class StockedCollection<TItem>
{
    private readonly List<(TItem Product, int Quantity)> _items;
    
    private readonly NewEntry<TItem> _newEntryDelegate;
    private readonly EqualityDelegate<TItem> _equalityDelegate;

    public StockedCollection(EqualityDelegate<TItem> equalityDelegate)
    {
        _items = [];

        _equalityDelegate = equalityDelegate;
        _newEntryDelegate = (item, quantity) => _items.Add((item, quantity));
    }

    internal NewEntry<TItem> NewEntry => _newEntryDelegate;
    internal int CountDistinct => _items.Count;
    internal int CountTotal
    {
        get
        {
            int total = 0;

            for (int i = 0; i < _items.Count; ++i)
                total += _items[i].Quantity;

            return total;
        }
    }

    internal (TItem Product, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Delete(TItem product)
    {   
        for (int i = 0; i < _items.Count; ++i)
        {
            if (_equalityDelegate(_items[i].Product, product))
            {
                _items.RemoveAt(i);
                return;
            }
        }
    }

    internal ReadOnlyCollection<(TItem Product, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());

    internal int CountOf(TItem product)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equalityDelegate(Product, product)) return Quantity;
        }

        return 0;
    }

    internal bool Contains(TItem product, EqualityDelegate<TItem> equalityDelegate)
    {
        Iteration((TItem p, ref int q) =>
        {
            if(_equalityDelegate(p, product)) return;
            return;
        });

        return true;
    }

    internal delegate void Body(TItem item, ref int quantity);
    internal Action<Body> Iteration => 
    (Body iterationBody) =>
    {
        for(int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];

            iterationBody(Product, ref Quantity);
        }
    }; 
}