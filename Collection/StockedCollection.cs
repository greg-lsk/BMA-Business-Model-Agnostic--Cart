using System.Collections.ObjectModel;

namespace Cart;

internal class StockedCollection<TItem>(EqualityDelegate<TItem> equalityDelegate)
{
    private readonly List<(TItem Product, int Quantity)> _items = [];
    private readonly EqualityDelegate<TItem> _equalityDelegate = equalityDelegate;

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

    internal AddMiddleware Add(TItem product, int quantity) => new();
    

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

    internal void UpdateQuantity(TItem product, QuantityUpdateDelegate update)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equalityDelegate(Product, product))
            {
                _items[i] = (Product, update(Quantity));
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
        return IterativeCheck((p, q) => equalityDelegate(p, product));
    }


    internal delegate bool CurrentCondition(TItem product, int quantity = 0);
    internal bool IterativeCheck(CurrentCondition check)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];

            if(check(Product, Quantity)) return true;
       }

        /*Do something here on miss*/
        return false;
    } 
}