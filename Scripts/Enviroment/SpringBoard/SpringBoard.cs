using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpringBoard : PoolObj
{

    private bool _isActive;
    public override bool IsActive { get { return _isActive; } }

    public override void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
       
    }

    private void OnEnable()
    {
        SetActive(true);

    }
}
