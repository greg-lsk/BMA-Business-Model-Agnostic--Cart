﻿namespace Cart;

internal class CartItems<TProduct>(List<CartItem<TProduct>> items)
{
    private readonly List<CartItem<TProduct>> _items = items;

    internal void Add(TProduct product, int quantity = 0) => _items.Add(new CartItem<TProduct>(product, quantity));
}