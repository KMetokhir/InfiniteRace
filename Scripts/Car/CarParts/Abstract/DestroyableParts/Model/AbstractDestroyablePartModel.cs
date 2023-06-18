using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDestroyablePartModel 
{
    public event Action PartFixedEvent;
    public event Action PartDestroiedEvent;
   
    private bool _isDestroied;    
  
    private Transform _parent;
    private Rigidbody _destroyedPartRB;

    private Vector3 _startLocalPosition;
    private Quaternion _startLocalRotation;

    public AbstractDestroyablePartModel(Rigidbody destroyedPartRB)
    {
        _destroyedPartRB = destroyedPartRB;

        _parent = _destroyedPartRB.transform.parent;

        _startLocalPosition = _destroyedPartRB.transform.localPosition;
        _startLocalRotation = _destroyedPartRB.transform.localRotation;
        
    }

    public void Destroy(Vector3 carCentrPoint, float forceMagnitude, Vector3 directionOffset)
    {
        if (_isDestroied)
            return;

        _isDestroied = true;

        PartDestroiedEvent?.Invoke();       

        _destroyedPartRB.transform.parent = null;       

        var direction = (_destroyedPartRB.transform.position - new Vector3(carCentrPoint.x, carCentrPoint.y, _destroyedPartRB.transform.position.z)).normalized;
        
        direction = (direction + directionOffset).normalized;

        _destroyedPartRB.isKinematic = false;
        _destroyedPartRB.useGravity = true;

        _destroyedPartRB.AddForce(direction * forceMagnitude, ForceMode.Impulse);
        _destroyedPartRB.AddTorque(direction * forceMagnitude, ForceMode.Impulse);

    }
    

    public void Fix()
    {

        if (!_isDestroied)
            return;

        _isDestroied = false;

        PartFixedEvent?.Invoke();

        _destroyedPartRB.isKinematic = true;
        _destroyedPartRB.useGravity = false;

        _destroyedPartRB.transform.parent = _parent.transform;
        _destroyedPartRB.transform.localPosition = _startLocalPosition;
        _destroyedPartRB.transform.localRotation = _startLocalRotation;

    }

}
