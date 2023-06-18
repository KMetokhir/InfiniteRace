using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDestroyedCarPartView : MonoBehaviour
{
    private MeshRenderer _destroyedPartMesh;
    private BoxCollider _collider;

    private void Awake()
    {
        _destroyedPartMesh = GetComponent<MeshRenderer>();
        _collider = GetComponent<BoxCollider>();
    }

    public void SetEnable(bool isEnabled)
    {
        _destroyedPartMesh.enabled = isEnabled;
        _collider.enabled = isEnabled;
    }
}
