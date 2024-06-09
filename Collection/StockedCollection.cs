using Utils;
using System.Collections.ObjectModel;

namespace Cart;

internal delegate void NewEntry<TItem>(TItem entity, int quantity);  
internal class StockedCollection<TItem>
{
    private readonly List<(TItem Item, int Quantity)> _items;
    
    private readonly NewEntry<TItem> _newEntryDelegate;
    private readonly EqualityDelegate<TItem> _equals;

    public StockedCollection(EqualityDelegate<TItem> equals)
    {
        _items = [];

        _equals = equalityDelegate;
        _newEntryDelegate = (item, quantity) => _items.Add((item, quantity));
    }

    internal NewEntry<TItem> NewEntry => _newEntry;
    internal int CountDistinct => _items.Count;
    internal int CountTotal
    {
        get
        {
            int total = 0;

            Iteration.On(_items, i => total += i.Current.Quantity);

            return total;
        }
    }

    internal (TItem Item, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Delete(TItem product) =>
    Iteration.On(_items, i =>  
    {
        if(!_equalityDelegate(i.Current.Product, product)) return;

        _items.Remove(i.Current);
        i.Break();
    });
    // {   
    //     for (int i = 0; i < _items.Count; ++i)
    //     {
    //         if (_equalityDelegate(_items[i].Product, product))
    //         {
    //             _items.RemoveAt(i);
    //             return;
    //         }
    //     }
    // }

    internal int CountOf(TItem product) =>
    Iteration.On(_items, i =>
    {
        var (Product, Quantity) = i.Current;

        if(_equalityDelegate(Product, product)) return (Quantity, Operation.Seize);

        return (0, Operation.Finished);
    });
    
    internal bool Contains(TItem product, EqualityDelegate<TItem> equalityDelegate) =>        
    Iteration.On(_items, i =>
    {
        var (Item, Quantity) = i.Current;

        if(_equalityDelegate(Item, product)) return (true, Operation.Seize);

        return (false, Operation.Finished);
    });
    
    internal ReadOnlyCollection<(TItem Item, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());
}