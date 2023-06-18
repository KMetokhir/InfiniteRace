using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarModel 
{
    public event Action<float> FrontWheelTurnEvent;
    public event Action<bool, float> SlidingEvent;
    public event Action<float, float> AccelerationInputChangeEvent;
    public event Action FallOnGroundEvent;  

    private bool _isFaling;

    private CarSettings _carSettings;
    private Transform _centrOfMass;
  
    private Rigidbody _rigidbody;
    private GroundChecker _groundChecker;
    private Transform _carTransform;

    private float _accelerationInput;
    private float _dragOnGround;
    private Vector3 _lastPosition;

    private bool _isSliding;   
    private float _maxInputMagnitude=100f; 


    public  CarModel(Transform carTransform, Transform centrOfMass, Rigidbody rigidbody, GroundChecker groundChecker, CarSettings carSettings)
    {                 
        _rigidbody = rigidbody;
        _groundChecker = groundChecker;
        _centrOfMass = centrOfMass;
        _carTransform = carTransform;
        _carSettings = carSettings;

        _rigidbody.mass = _carSettings.RBMass;
        _rigidbody.drag = _carSettings.RBDrag;
        _rigidbody.angularDrag = _carSettings.RBAngularDrag;

        _dragOnGround = rigidbody.drag;
        _rigidbody.centerOfMass = _centrOfMass.position;
       
    }      

    public  void Enable()
    {
        _groundChecker.CarOnTheGroundEvent += OnCarOnTheGround;
        _groundChecker.CarNotOnTheGroundEvent += OnCarNotOnTheGround;
        _groundChecker.OneWheelOnTheAir += OnOnewheelOnTheAir;

    }
       

    public void Disable()
    {
       _groundChecker.CarOnTheGroundEvent -= OnCarOnTheGround;
        _groundChecker.CarNotOnTheGroundEvent -= OnCarNotOnTheGround;
        _groundChecker.OneWheelOnTheAir -= OnOnewheelOnTheAir;
    }


    public  void StopedByDeath()
    {       
         
        _rigidbody.constraints = RigidbodyConstraints.None; 
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.drag = 2f;

        _rigidbody.AddForce(Vector3.up * 200f, ForceMode.Impulse);      

    }


    private void OnOnewheelOnTheAir()
    {
        SlidingEvent?.Invoke(false, 0);       
    }


    private void OnCarOnTheGround()
    {
     
        IsSlidingCheck();        
        var targetRot = Vector3.Lerp(_carTransform.up, _groundChecker.NormalTarget, Time.fixedDeltaTime * 30f);      
        _rigidbody.MoveRotation(Quaternion.FromToRotation(_carTransform.up, targetRot) * _carTransform.rotation);

        if (_isFaling)
        {
            FallOnGroundEvent?.Invoke();
            _isFaling = false;
        }
    }

    private void OnCarNotOnTheGround()
    {
        
        SlidingEvent?.Invoke(false, 0);
       
        if (_rigidbody.velocity.magnitude < 0.2f)
        {
            _isFaling = false;
            _rigidbody.AddForce(_rigidbody.transform.forward * 1000f, ForceMode.Force);         
        }
        else {
            _isFaling = true;       
        }

        if (_isFaling)
        {
            _rigidbody.MoveRotation(Quaternion.FromToRotation(_carTransform.forward, _carTransform.forward+new Vector3(0,-0.01f,0)) * _carTransform.rotation);
        }

    }

    #region "FrontBackMove"

    public void MoveForvard()
    {       

         _accelerationInput = Mathf.Lerp(_accelerationInput, _carSettings.Acceleration, Time.fixedDeltaTime * _carSettings.AccelerationModifire) * 1;
       
        Move(_groundChecker.IsGrounded, _rigidbody);       

    }

    public void Brake()
    {
        _accelerationInput = 0;
        Move(_groundChecker.IsGrounded, _rigidbody);

        AccelerationInputChangeEvent?.Invoke(_accelerationInput, _carSettings.Acceleration);

    }
   

    public void MoveBackward()
    {       
        _accelerationInput = Mathf.Lerp(Mathf.Abs(_accelerationInput), _carSettings.Acceleration, Time.fixedDeltaTime * _carSettings.AccelerationModifire)*-1;
        Move(_groundChecker.IsGrounded, _rigidbody);
      
    }

    private void Move(bool isGrounded, Rigidbody rb)
    {
     

        if (isGrounded)
        {
            rb.drag = _dragOnGround;
            rb.AddRelativeForce(Vector3.forward * _accelerationInput); // движение вперед назад
         
        }
        else
        {
            rb.drag = .1f;
            rb.AddForce(-Vector3.up * _carSettings.GravityMod * 20f);
        }

        if (rb.velocity.magnitude >= _carSettings.MaxVelocity)
        {
            rb.velocity = rb.velocity.normalized * _carSettings.MaxVelocity;
        }

      
    }

    #endregion

    #region "LeftRightTurn"

    public void TurnLeft(float magnitude)
    {      
        var direction = new Vector3(0, 0, 1f);        
        var rotation = Quaternion.Euler(0, -60, 0);
        direction = rotation* direction;
        
        Turn(direction, magnitude, _groundChecker.IsGrounded, _rigidbody);      
    }

    public void TurnRight(float magnitude)
    {     
        var direction = new Vector3(0, 0, 1f);
        var rotation = Quaternion.Euler(0,60, 0);
        direction = rotation * direction;

        Turn(direction, magnitude, _groundChecker.IsGrounded, _rigidbody);     
    }

    private void Turn(Vector3 direction, float magnitude, bool isGrounded, Rigidbody rb) 
    {                                                                    
        if (!isGrounded)
            return;       

        magnitude = magnitude * 1000f;       
        magnitude = Mathf.Clamp(magnitude, 0f, _maxInputMagnitude);

        var target = direction;
        
        float rotSpeed = rb.velocity.magnitude / 2000f;
        float sideSpeed = rb.velocity.magnitude * magnitude / _carSettings.SideSpeedDecreaser;
       
        var targetRot = Quaternion.Lerp(_carTransform.rotation,
         Quaternion.LookRotation(target, Vector3.up), magnitude * Time.fixedDeltaTime * Mathf.Clamp(rotSpeed, 0, 1) * _carSettings.RotationSpeed);

        rb.MoveRotation(targetRot);
        rb.AddRelativeForce(target * sideSpeed);

    }

    #endregion

    private void IsSlidingCheck()
    {
        Vector3 direction = _carTransform.position - _lastPosition;
        Vector3 movement = _carTransform.InverseTransformDirection(direction);
        _lastPosition = _carTransform.position;

        float intencity = Mathf.Abs(movement.x);

      
        if (Mathf.Abs(_accelerationInput) < _carSettings.Acceleration * 0.6f && _accelerationInput != 0)
        {
            _isSliding = true;
            SlidingEvent?.Invoke(_isSliding, 0.05f);

        }
        else if (intencity > _carSettings.SlidingLevel)
        {
            _isSliding = true;
            SlidingEvent?.Invoke(_isSliding, intencity);

        }
        else 
        {
            _isSliding = false;
             SlidingEvent?.Invoke(_isSliding, intencity);
        }            
      
    }  
    
}
