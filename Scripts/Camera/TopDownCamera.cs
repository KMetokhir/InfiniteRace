using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TopDownCamera : MonoBehaviour
{
    public float CameraHeight => _cameraHeight;

    [SerializeField] private Transform GroundLevel;
    [SerializeField] private float _aheadSpeed;
    [SerializeField] private float _cameraHeight;
    [SerializeField] private float _followDamping;
    [SerializeField] private float _zOfSet;

    [Inject] private DynamicChassis _carDynamicChassis;

    private Transform _observable;
    private Rigidbody _observableRigidbody;
    private Vector3 _targetPosition;
    

    private void Start()
    {       
        _observableRigidbody = _carDynamicChassis.GetComponent<Rigidbody>();
        _observable = _carDynamicChassis.transform;
        _targetPosition = new Vector3(GroundLevel.transform.position.x, GroundLevel.transform.position.y, _observable.position.z) + Vector3.up * _cameraHeight;  
    }

    private void Update()
    {
        if (_observable == null)
            return;
       
        _zOfSet = _observableRigidbody.velocity.z * _aheadSpeed;
        _zOfSet = Mathf.Clamp(_zOfSet, 0, 57f);

        _targetPosition = new Vector3(_targetPosition.x, _targetPosition.y, _observable.position.z+ _zOfSet);

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _followDamping);
    }

}
