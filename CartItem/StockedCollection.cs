using System.Collections.ObjectModel;

namespace Cart;

internal class StockedCollection<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly List<(TProduct Product, int Quantity)> _items = [];

    private readonly EqualityDelegate<TProduct> _equals = equalityDelegate;

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

    internal (TProduct Product, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Add(TProduct product,
                      int quantity,
                      Func<bool> updateCondition,
                      Action updateAction)
    {
        if (updateCondition()) updateAction();
        else _items.Add((product, quantity));
    }

    internal void Delete(TProduct product)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            if (_equals(_items[i].Product, product))
            {
                _items.RemoveAt(i);
                return;
            }
        }
    }

    internal void UpdateQuantity(TProduct product, QuantityUpdateDelegate update)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equals(Product, product))
            {
                _items[i] = (Product, update(Quantity));
                return;
            }
        }
    }

    internal ReadOnlyCollection<(TProduct Product, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());

    internal int CountOf(TProduct product)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equals(Product, product)) return Quantity;
        }

        return 0;
    }

    internal bool Contains(TProduct product, EqualityDelegate<TProduct>? equalityDelegate = null)
    {
        equalityDelegate ??= _equals;

        for (int i = 0; i < _items.Count; ++i)
        {
            if (equalityDelegate(_items[i].Product, product)) return true;
        }

        return false;
    }

    internal bool IterativeCheck(Func<TProduct, int, bool> check)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (check(Product, Quantity)) /*do something here, on hit*/ return true;
        }

        /*Do something here on miss*/
        return false;
    } 
}