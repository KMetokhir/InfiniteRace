using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusModel 
{
    public event Action CollectedEvent;
    public event Action<int> ValueSetEvent;

    private int _bonusValue; 
    private Vector2Int _valueRange;   


    public BonusModel(Vector2Int valueRange)
    {
        _valueRange = valueRange;

        _bonusValue = UnityEngine.Random.Range(_valueRange.x, _valueRange.y);
    }

    public void Enable()
    {
        ValueSetEvent?.Invoke(_bonusValue);
    }

    public int Collect()
    {
        CollectedEvent?.Invoke();
        return _bonusValue;
    }    

}
