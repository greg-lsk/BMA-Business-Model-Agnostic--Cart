﻿using System.Collections.ObjectModel;

namespace Cart;

public delegate bool EqualityDelegate<TProduct>(TProduct product1, TProduct product2);
public delegate int QuantityUpdateDelegate(int inCartQuantity);

public class Cart<TProduct>(EqualityDelegate<TProduct> equalityDelegate)
{
    private readonly StockedCollection<TProduct> _items = new(equalityDelegate);
    private EqualityDelegate<TProduct> _equalityDelegate = equalityDelegate;

    public int CountDistinct => _items.CountDistinct;
    public int CountTotal => _items.CountTotal;

    public bool Contains(TProduct product) => _items.Contains(product, _equalityDelegate); 

    public void Add(TProduct product, int quantity = 1) => AddMiddlewareBuilder.Initialize()
        .When(product, Contains)
        .Do(product, (p) => UpdateQuantity(p, i => i + quantity));
 
    public void Delete(TProduct product) => _items.Delete(product);
    public void UpdateQuantity(TProduct product, QuantityUpdateDelegate updateDelegate) 
        => _items.UpdateQuantity(product, updateDelegate);
    
    public ReadOnlyCollection<(TProduct Product, int Quantity)> Content() => _items.AsReadonly();

    //Pricing
}