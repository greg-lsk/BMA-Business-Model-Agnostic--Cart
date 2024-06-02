﻿using System.Collections.ObjectModel;

namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate int QuantityUpdateDelegate(int inCartQuantity);

public class Cart<TProduct>(ICondition equalityCondition)
{
    private readonly StockedCollection<TProduct> _items = new(equalityCondition);

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    private EqualityDelegate<TProduct> _equalityDelegate;
    private ICondition _equality = equalityCondition;
    private ICondition _defaultEquality = new ConditionPipe<TProduct>()
        .AddCondition(p => _equalityDelegate(p, ));

    public void Add(TProduct product, int quantity = 1) => 
    _items.Add
    (
        product, 
        quantity, 
        () => _items.Contains(product, _equality), 
        () => _items.UpdateQuantity(product, inCart => inCart + quantity) 
    );
 
    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateDelegate) 
        => _items.UpdateQuantity(product, updateDelegate);
    
    public ReadOnlyCollection<(TProduct Product, int Quantity)> Content() => _items.AsReadonly();

    //Pricing
}