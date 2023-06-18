using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentView : MonoBehaviour
{
    public event Action SegmentViewBecameInvisibleEvent;   

    [SerializeField] private float _scaleFactor;

    private Material _mat;


    private MeshRenderer _meshR;

    private void Awake()
    {
        _meshR = GetComponent<MeshRenderer>();
        _mat = GetComponent<Renderer>().material;

        _mat.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.z) / _scaleFactor;
    }

    private void Update()
    {
       // if (transform.hasChanged)
        {
            _mat.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.z)/_scaleFactor;           
            transform.hasChanged = false;
        }
    }

    private void OnBecameInvisible()
    {
        SegmentViewBecameInvisibleEvent?.Invoke();  
    }

    public void SetVisible(bool isVisible)
    {
        _meshR.enabled= isVisible;
    }

    public void AddCollider()
    {
       gameObject.AddComponent<MeshCollider>();
    }
}
