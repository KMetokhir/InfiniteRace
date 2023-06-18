using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionTracker : MonoBehaviour
{
   
    public event Action<IDamageable> DamagiableCollidedEvent;
    public event Action<IBonus> BonusCollidedEvent;

    private Collider _currentCollision;


    private void OnTriggerEnter(Collider other)
    {
        if (other == _currentCollision)
            return;
        else
            _currentCollision = other;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            DamagiableCollidedEvent?.Invoke(damageable);          
        }

        if (other.TryGetComponent(out IBonus bonus))
        {
            BonusCollidedEvent?.Invoke(bonus);
        }

    }
}
