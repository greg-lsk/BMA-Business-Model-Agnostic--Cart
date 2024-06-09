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

    internal void Delete(TItem item) => 
    Iteration.On(_items, i => 
    {
        if(!_equals(i.Current.Item, item)) return Operation.Continue;
                 
        _items.Remove(i.Current); 
        return Operation.Break;
        
    });

    internal int QuantityOf(TItem item) =>
    Iteration.On(_items, i => _equals(i.Current.Item, item) switch
    {
        true  => (i.Current.Quantity, Operation.Break),
        false => (0, Operation.Continue) 
    });
    
    internal bool Contains(TItem item, EqualityDelegate<TItem> equalityDelegate) =>        
    Iteration.On(_items, i => _equals(i.Current.Item, item) switch
    {
        true  => (true, Operation.Break),
        false => (false, Operation.Continue)
    });
    
    internal ReadOnlyCollection<(TItem Item, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());
}