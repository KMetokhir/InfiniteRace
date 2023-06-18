using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Durability: IDurabilityEvents
{
    public event Action<int> ValueChangedEvent;
    public event Action DeadEvent;
    public int Value { get; private set; }

    public Durability(int value)
    {
        Value = value;
    }

    public void IncreaseValue(int value)
    {
        Value += value;
        ValueChangedEvent?.Invoke(Value);     
    }

    public void DecreaseValue(int value)
    {
        Value -= value;
     
        if (Value <= 0)
        {
            Value = 0;   
            DeadEvent?.Invoke();
        }
        ValueChangedEvent?.Invoke(Value);

    }

    public void Enable()
    {
        ValueChangedEvent?.Invoke(Value);
    }
}
