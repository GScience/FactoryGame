using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int size;

    private Item[] _items;

    void Start()
    {
        _items = new Item[size];
    }

    public Item GetItem(int index)
    {
        return _items[index];
    }

    public bool TryTakeItem(Item item)
    {
        for (var i = 0; i < _items.Length; ++i)
        {
            if (_items[i] != item)
                continue;
            _items[i] = null;
            return true;
        }

        return false;
    }

    public bool TryAddItem(Item item)
    {
        for (var i = 0; i < _items.Length; ++i)
        {
            if (_items[i] != null)
                continue;
            _items[i] = item;
            return true;
        }

        return false;
    }

    public void OnValidate()
    {
        if (size < 0)
            size = 0;
    }
}
