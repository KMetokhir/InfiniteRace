using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : OpenablePart
{
    [SerializeField] private float _maxOpenAngle;


    private void Awake()
    {
        if (_maxOpenAngle <= 0)
        {
            Debug.LogError("MaxOpenAngle can't be equal or less Zerro");
            _maxOpenAngle = Mathf.Clamp(_maxOpenAngle, 1f, 180f);
        }

        DoorView view = GetComponentInChildren<DoorView>();
        DestroyedDoorView destroyedView = GetComponentInChildren<DestroyedDoorView>();

        Rigidbody destroyedRB = destroyedView.GetComponent<Rigidbody>();
        Rigidbody rb = GetComponent<Rigidbody>();
        HingeJoint joint = GetComponent<HingeJoint>();

        DoorModel model = new DoorModel(rb, destroyedRB, joint, _maxOpenAngle);

        InitializeOpenablePart(model, view, destroyedView);
      
    }   

}
