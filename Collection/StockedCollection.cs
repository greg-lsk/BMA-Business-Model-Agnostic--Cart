using System.Collections.ObjectModel;

namespace Cart;

internal delegate TRefProperty RefSelector<TProduct, TRefProperty>(TProduct from);
internal delegate bool CheckDelegate<TProduct, TRefProperty>(
    RefSelector<TProduct, TRefProperty> selector, 
    Predicate<TRefProperty> condition);

internal class StockedCollection<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly List<(TProduct Product, int Quantity)> _items = [];
    private readonly EqualityDelegate<TProduct> _equalityDelegate = equalityDelegate;

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

    internal readonly struct AddMiddleware(bool state = true)
    {
        private readonly bool _state = state;

        internal AddMiddleware When<TSubject>(TSubject subject, Func<TSubject, bool> cond)
        {
            if(!_state) return new(false);

            if( !cond(subject) ) return new(true);
            else return new(false);
        }

        internal AddMiddleware Do<TSubject>(TSubject subject, Action<TSubject> action)
        {
            if(!_state) return new(false);

            action(subject);
            return this;
        }
    }
    internal readonly struct AddMiddlewareBuilder
    {
        internal static AddMiddleware Initialize() => new();
    }

    internal AddMiddleware Add(TProduct product, int quantity) => new();
    

    internal void Delete(TProduct product)
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

    internal void UpdateQuantity(TProduct product, QuantityUpdateDelegate update)
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

    internal ReadOnlyCollection<(TProduct Product, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());

    internal int CountOf(TProduct product)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equalityDelegate(Product, product)) return Quantity;
        }

        return 0;
    }

    internal bool Contains(TProduct product, EqualityDelegate<TProduct> equalityDelegate)
    {
        return IterativeCheck((p, q) => equalityDelegate(p, product));
    }


    internal delegate bool CurrentCondition(TProduct product, int quantity = 0);
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