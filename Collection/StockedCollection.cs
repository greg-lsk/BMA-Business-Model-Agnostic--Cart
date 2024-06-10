using Utils;
using System.Collections.ObjectModel;

namespace Cart;

internal delegate void NewEntry<TItem>(TItem entity, int quantity);  
internal class StockedCollection<TItem>
{
    private readonly List<(TItem Item, int Quantity)> _items;
    
    private readonly NewEntry<TItem> _newEntry;
    private readonly EqualityDelegate<TItem> _equals;

    public StockedCollection(EqualityDelegate<TItem> equals)
    {
        _items = [];

        _equals = equals;
        _newEntry = (i, q) => _items.Add((i, q));
    }

    internal NewEntry<TItem> NewEntry => _newEntry;
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
    internal (TItem Item, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Delete(TItem item) =>
    Iteration.On(_items, i =>  
    {
        if(_equals(i.Current.Item, item))
        {
            _items.Remove(i.Current);
            i.Break();
        }    
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

    internal int CountOf(TItem item) =>
    Iteration.On(_items).Run<int>((t, i) => 
    {
        if(_equals(i.Current.Item, item))
        {
            t.Capture(i.Current.Quantity);
            i.Break();
        }
    });
    
    internal bool Contains(TItem item, EqualityDelegate<TItem> equalityDelegate) => 
    Iteration.On(_items).Run<bool>((t, i) =>
    {
        var found = _equals(i.Current.Item, item);

        if(_equalityDelegate(Item, product)) return (true, Operation.Break);

        return (false, Operation.Continue);
    });
    
    internal ReadOnlyCollection<(TItem Item, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());
}