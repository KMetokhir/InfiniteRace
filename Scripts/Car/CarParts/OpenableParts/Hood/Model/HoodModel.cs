using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodModel : OpenablePartModel
{

    public HoodModel(Rigidbody rb, Rigidbody destroyedPartRB, HingeJoint joint, float maxOpenAngle) :base( rb, joint, maxOpenAngle, destroyedPartRB)
    { }

  
}
