using Utils;
using System.Collections.ObjectModel;

namespace Cart;

internal delegate void NewEntry<TEntity>(TEntity entity, int quantity);  
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
    internal int CountDistinct => _items.Count;
    internal int CountTotal
    {
        get
        {
            int total = 0;
            Iteration.On(_items).Run(i => total += i.Quantity);
            return total;
        }
    }

    internal (TItem Item, int Quantity) this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    internal void Remove(TItem item) => Iteration.On(_items)
                                                 .When(i => _equals(i.Item, item))
                                                 .Run(_items.Remove);
            
    internal int? CountOf(TItem item) 
    => Iteration.On(_items)
                .Apply(i => _equals(i.Item, item))
                .Map(e => e switch{
                    true  => new(i => i.Quantity),
                    false => default
                },  none: () => default(int?));
        
    internal bool Contains(TItem item, EqualityDelegate<TItem> equalityDelegate) 
    => Iteration.On(_items)
                .Apply(i => _equals(i.Item, item))
                .Map(e => e switch{
                    true  => new(true),
                    false => default
                }, none: () => false);
    
    internal ReadOnlyCollection<(TItem Product, int Quantity)> AsReadonly() => Array.AsReadOnly(_items.ToArray());
}