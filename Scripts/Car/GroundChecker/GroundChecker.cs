using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
   
    public event UnityAction CarOnTheGroundEvent;
    public event UnityAction CarNotOnTheGroundEvent;
    public event UnityAction OneWheelOnTheAir;

    public bool IsGrounded { get; private set; }
    public   Vector3 NormalTarget { get; private set; }

    [SerializeField] private Transform _groundRayPoint1R;
    [SerializeField] private Transform _groundRayPoint1L;
    [SerializeField] private Transform _groundRayPoint2R;
    [SerializeField] private Transform _groundRayPoint2L;
    [SerializeField] private float groundRayLength = 1f;


    private LayerMask _groundLayerMask;   
  

    private void Awake() 
    {       
        _groundLayerMask = LayerMask.GetMask("Ground");       
    }   

    private void Update()
    {             
        IsGrounded = false;
        RaycastHit hit;
        NormalTarget = Vector3.zero;
        Debug.DrawRay(_groundRayPoint1R.position, Vector3.down * groundRayLength, Color.red);

        if (Physics.Raycast(_groundRayPoint1R.position, Vector3.down, out hit, groundRayLength, _groundLayerMask ))
         {
             IsGrounded = true;

             NormalTarget = hit.normal;
        }
        else
        {
            OneWheelOnTheAir?.Invoke();
        }


        if (Physics.Raycast(_groundRayPoint1L.position, Vector3.down, out hit, groundRayLength, _groundLayerMask ))
        {
            IsGrounded = true;

            NormalTarget = hit.normal;

        }
        else {
            OneWheelOnTheAir?.Invoke();
        }


        if (Physics.Raycast(_groundRayPoint2R.position, Vector3.down, out hit, groundRayLength, _groundLayerMask ))
         {
             IsGrounded = true;

             NormalTarget = ((NormalTarget + hit.normal) / 2f).normalized;
         
        }
        else
        {
            OneWheelOnTheAir?.Invoke();
        }


        if (Physics.Raycast(_groundRayPoint2L.position, Vector3.down, out hit, groundRayLength, _groundLayerMask ))
        {
            IsGrounded = true;

            NormalTarget = (NormalTarget + hit.normal) / 2f;
        
        }
        else
        {
            OneWheelOnTheAir?.Invoke();
        }



        if (IsGrounded)
        {          
              CarOnTheGroundEvent?.Invoke();
         }
         else
        {          
            CarNotOnTheGroundEvent?.Invoke();
        }
    }   

}
