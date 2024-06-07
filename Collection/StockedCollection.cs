using Utils;
using System.Collections.ObjectModel;

namespace Cart;

internal delegate void NewEntry<TItem>(TItem entity, int quantity);  
internal class StockedCollection<TItem>
{
    private readonly List<(TItem Item, int Quantity)> _items;
    
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

            Iteration.For(_items, i =>
            {
                var (Item, Quantity) = i.Current;
                total += Quantity;
            });

            return total;
        }
    }

    internal (TItem Product, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Delete(TItem item)
    {   
        for (int i = 0; i < _items.Count; ++i)
        {
            if (_equalityDelegate(_items[i].Item, item))
            {
                _items.RemoveAt(i);
                return;
            }
        }
    }

    internal int QuantityOf(TItem item) =>
    Iteration.On(_items, i => _equalityDelegate(i.Current.Item, item) switch
    {
        true  => (i.Current.Quantity, Operation.Break),
        false => (0, Operation.Continue) 
    });
    
    internal bool Contains(TItem item, EqualityDelegate<TItem> equalityDelegate) =>        
    Iteration.On(_items, i => _equalityDelegate(i.Current.Item, item) switch
    {
        true  => (true, Operation.Break),
        false => (false, Operation.Continue)
    });
    
    internal ReadOnlyCollection<(TItem Item, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());
}