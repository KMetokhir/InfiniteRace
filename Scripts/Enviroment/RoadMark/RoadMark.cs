using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMark : MonoBehaviour
{ 

    private LineRenderer _lineRenderer;  

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Setscale(float width, float length)
    {
        _lineRenderer.SetPosition(1, new Vector3(0, 0.0001f, length));
        _lineRenderer.widthMultiplier = width;
    }
   
}
