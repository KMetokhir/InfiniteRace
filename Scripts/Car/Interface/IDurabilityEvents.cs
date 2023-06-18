using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDurabilityEvents 
{
    public event Action<int> ValueChangedEvent;
    public event Action DeadEvent;
}
