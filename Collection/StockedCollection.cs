﻿using Utils;
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
            Iteration.On(_items).Run((ref Iterator<(TItem Item, int Quantity)> i) => total += i.Current.Quantity);
            return total;
        }
    }

    internal (TItem Item, int Quantity) this[int index]
    internal (TItem Item, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Remove(TItem item) => 
    Iteration.On(_items)
             .WhenMet(i => _equals(i.Item, item), i => _items.Remove(i));
    
    internal int CountOf(TItem item) =>
    Iteration.On(_items).Run((ref Iterator<(TItem Item, int Quantity)> i) =>
    {
        if(_equals(i.Current.Item, item))
        {
            i.Break();
            return i.Current.Quantity;
        }
        return 0;
    });
    
    internal bool Contains(TItem item, EqualityDelegate<TItem> equalityDelegate) => 
    Iteration.On(_items).Run((ref Iterator<(TItem Item, int Quantity)> i) =>
    {
        var found = _equals(i.Current.Item, item);

        if(found)
        {
            t.Capture(found);
            i.Break();
        }
    });
    
    internal ReadOnlyCollection<(TItem Item, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());
}