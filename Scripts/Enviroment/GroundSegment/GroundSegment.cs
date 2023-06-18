using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSegment : MonoBehaviour
{
    public event Action<GroundSegment> SegmentBecameInvisibleEvent;

    public SegmentView View { get; private set; }
   

    private void Awake()
    {
        View = GetComponentInChildren<SegmentView>(); 
    }

    private void OnEnable()
    {
        View.SegmentViewBecameInvisibleEvent += OnSegmentBecameInvisible;
    }

    private void OnSegmentBecameInvisible()
    {
        SegmentBecameInvisibleEvent?.Invoke(this);
    }   

}
