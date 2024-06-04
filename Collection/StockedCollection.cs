using System.Collections.ObjectModel;

namespace Cart;

internal delegate TRefProperty RefSelector<TProduct, TRefProperty>(TProduct from);
internal delegate bool CheckDelegate<TProduct, TRefProperty>(
    RefSelector<TProduct, TRefProperty> selector, 
    Predicate<TRefProperty> condition);

internal class StockedCollection<TProduct>(ICondition equality)
{
    private readonly List<(TProduct Product, int Quantity)> _items = [];

    private readonly ICondition _equality = equality;

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

    internal readonly struct AddMiddleware
    {
        internal AddMiddleware WhenNot<TSubject>(Func<TSubject, bool> cond, TSubject subject)
        {
            if( !cond(subject) ) return this;
            else return this;
        }
    }
    internal AddMiddleware Add(TProduct product, int quantity) => new();
    

    internal void Delete(TProduct product)
    {
        var provider = new ParameterProvider();
        provider[0] = product;
        
        for (int i = 0; i < _items.Count; ++i)
        {
            if (_equality.AppliesTo(_items[i].Product, provider))
            {
                _items.RemoveAt(i);
                return;
            }
        }
    }

    internal void UpdateQuantity(TProduct product, QuantityUpdateDelegate update)
    {
        var context = new ParameterContext<TProduct>(product);

        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equality.AppliesTo(Product, context))
            {
                _items[i] = (Product, update(Quantity));
                return;
            }
        }
    }

    internal ReadOnlyCollection<(TProduct Product, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());

    internal int CountOf(TProduct product)
    {
        var context = new ParameterContext<TProduct>(product);        

        for (int i = 0; i < _items.Count; ++i)
        {
            var (Product, Quantity) = _items[i];
            if (_equality.AppliesTo(Product, context)) return Quantity;
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