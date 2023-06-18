using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Settings", menuName = "Car/Settings", order = 51)]
public class CarSettings : ScriptableObject
{
    [SerializeField] private int _startDurabilityLevel = 10;
    [SerializeField] private float _acceleration = 17000f;
    [SerializeField] private float _accelerationModifire = 1f;
    [SerializeField] private float _maxVelocity = 30f;
    [SerializeField] private float _rotationSpeed = 4f;
    [SerializeField] private float _gravityMod = 10f;
    [SerializeField] private float _sideSpeedDecreaser = 5f;
    [SerializeField] private float _slidingLevel = 0.01f;

        
    [SerializeField] private float _rbMass;
    [SerializeField] private float _rbDrag;
    [SerializeField] private float _rbAngularDrag;


    public int StartDurabilityLevel => _startDurabilityLevel;
    public float Acceleration => _acceleration;
    public float AccelerationModifire => _accelerationModifire;
    public float MaxVelocity => _maxVelocity;
    public float RotationSpeed => _rotationSpeed;
    public float GravityMod => _gravityMod;
    public float SideSpeedDecreaser => _sideSpeedDecreaser;
    public float SlidingLevel => _slidingLevel;

    public float RBMass => _rbMass;
    public float RBDrag => _rbDrag;
    public float RBAngularDrag => _rbAngularDrag;
}
