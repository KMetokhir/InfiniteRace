using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObj : MonoBehaviour
{
    public Action< bool> ObjIsActiveEvent;

    public abstract bool IsActive { get;  }

    public abstract void SetActive(bool isActive);   
    
}
