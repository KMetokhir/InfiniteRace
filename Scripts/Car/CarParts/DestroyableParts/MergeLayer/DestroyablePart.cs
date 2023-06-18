using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyablePart : AbstractDestroyablePart
{
    private void Awake()
    {        
        CarPartView view = GetComponentInChildren<CarPartView>();    
        
        DestroyedPartView destroyedView = GetComponentInChildren<DestroyedPartView>();
        Rigidbody destroyedRB = destroyedView.GetComponent<Rigidbody>();       
        DestroyablePartModel model = new DestroyablePartModel(destroyedRB);

        InitializeDestroyblePart(model, view, destroyedView);
    }
}
