using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCarPartView : MonoBehaviour
{
    private MeshRenderer _carPartMesh;


    private void Awake()
    {
        _carPartMesh = GetComponent<MeshRenderer>();
    }

    public void SetEnable(bool isEnabled)
    {      
        _carPartMesh.enabled = isEnabled;
    }
}
