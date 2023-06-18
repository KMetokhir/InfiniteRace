using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorModel: OpenablePartModel
{
    public DoorModel(Rigidbody rb, Rigidbody destroyedPartRB, HingeJoint joint, float maxOpenAngle ) : base( rb,   joint, maxOpenAngle, destroyedPartRB)
    {
       
    }  

}
