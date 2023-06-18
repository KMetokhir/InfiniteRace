using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarInput : MonoBehaviour
{
      
    public event UnityAction PointerUpEvent;
    public UnityAction PointerTapEvent;
    public event UnityAction<float> PointerUpMoveEvent;
    public event UnityAction<float> PointerDownMoveEvent;
    public event UnityAction PointerStayEvent;
    public event UnityAction MouseB1Down;


    private Camera _camera;
    private Vector3 _previousPointerPosition;

    private void Awake()
    {
        _camera = Camera.main;
    }


    private void FixedUpdate()
    {
       if (Input.GetMouseButtonDown(0))
        {
            _previousPointerPosition = Input.mousePosition;          

        }        

        if (Input.GetMouseButton(0))
        {

            PointerTapEvent?.Invoke();


            if (_previousPointerPosition.y < Input.mousePosition.y)
            {

                PointerUpMoveEvent?.Invoke((_camera.ScreenToViewportPoint((Input.mousePosition - _previousPointerPosition))).magnitude);

            }
            else if (_previousPointerPosition.y > Input.mousePosition.y)
            {               
                PointerDownMoveEvent?.Invoke((_camera.ScreenToViewportPoint((Input.mousePosition - _previousPointerPosition))).magnitude);
            }
            else
            {
                PointerStayEvent?.Invoke();
            }          

        }
        else if(Input.GetMouseButton(1))
            {
                MouseB1Down?.Invoke();
            }

        else
        {
            PointerUpEvent?.Invoke();    
        }

        _previousPointerPosition = Input.mousePosition; 
       
    }
}
