using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockModel 
{
    public event Action DeadEvent;
    public event Action<int> DurabilityChangedEvent;
    public event Action<int, int> DestroyPriceSeted;

    private Vector2Int _destroyPriceRange;
    private int _destroyPrice;
    private int _durabilityLevel;

    public BlockModel(Vector2Int destroyPriceRange)
    {
        _destroyPriceRange = destroyPriceRange;
        _destroyPrice = UnityEngine.Random.Range(_destroyPriceRange.x, _destroyPriceRange.y + 1);

    }

    public  void Refresh()
    {
        _durabilityLevel = _destroyPrice;
        DurabilityChangedEvent?.Invoke(_durabilityLevel);
        DestroyPriceSeted?.Invoke(_destroyPrice, _destroyPriceRange.y);
    }   

     
    public int TakeDamage(int value)
    {

        _durabilityLevel -= value;
       
        if (_durabilityLevel<=0)
        {
            _durabilityLevel = 0;
            DurabilityChangedEvent?.Invoke(_durabilityLevel);
            DeadEvent?.Invoke();
            return _destroyPrice;
        }
        DurabilityChangedEvent?.Invoke(_durabilityLevel);      
        return _destroyPrice;
    }
}
