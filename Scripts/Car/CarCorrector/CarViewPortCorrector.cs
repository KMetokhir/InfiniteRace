using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CarViewPortCorrector : MonoBehaviour
{  
    [SerializeField] private float _xOffset;

    [Inject] private DynamicChassis _carDynamicChassis;

    private Rigidbody _rb;

    private void Awake()
    {       
        _rb = _carDynamicChassis.GetComponent<Rigidbody>();
    }
  
    private void Update()
    {
        if (Camera.main.WorldToViewportPoint(_carDynamicChassis.transform.position + new Vector3(0, _xOffset,0)).y  > 1)
        {   
            _rb.AddForce(Vector3.right* 1000f);
        }

        if (Camera.main.WorldToViewportPoint(_carDynamicChassis.transform.position + new Vector3(0, _xOffset, 0)).y < 0)
        {
           _rb.AddForce(Vector3.left *1000f);
        }

    }
}
