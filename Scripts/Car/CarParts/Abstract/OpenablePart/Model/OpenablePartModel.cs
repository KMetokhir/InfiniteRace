using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OpenablePartModel : AbstractDestroyablePartModel
{    
    private bool _isOpen;  

    private Rigidbody _rb; 
    private HingeJoint _joint;
  
    private float _maxOpenAngle; 
    


    public OpenablePartModel(Rigidbody rb,  HingeJoint joint, float maxOpenAngle, Rigidbody destroyedPartRB):base(destroyedPartRB)
    {
        _joint = joint;
        _rb = rb;

        _rb.isKinematic = false;
        _rb.useGravity = true;      

        _maxOpenAngle = maxOpenAngle;

        _joint = joint;
        JointLimits limits = new JointLimits();
        limits.max = 0;
        _joint.limits = limits;

        _isOpen = false;

    }

    public void Open()
    {
        if (_isOpen)
            return;

        _isOpen = true;

        JointLimits limits = _joint.limits;
        limits.max = _maxOpenAngle;
        _joint.limits = limits;
    }

  
    public void Close()
    {
        if ( !_isOpen)
            return;

        _isOpen = false;

        JointLimits limits = new JointLimits();
        limits.max = 0;
        _joint.limits = limits;

    }
    
}
