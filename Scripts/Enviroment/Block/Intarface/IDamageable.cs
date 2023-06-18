using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{   
    public int TakeDamage(int value, float carVelocity, float carMaxVelocity);
}
