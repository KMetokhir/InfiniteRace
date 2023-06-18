using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheelskid : MonoBehaviour
{ 
    [SerializeField] private float _intensityModifire=2f;

    private ParticleSystem _smogParticles;
    private Skidmarks _skidmarkController;
    private int _lastSkidId = -1;
    private bool isInitialized;


    private void Awake()
    {
        _smogParticles = GetComponent<ParticleSystem>();
    }

    public void Init(Skidmarks skidmarkController )
    {
        _skidmarkController = skidmarkController;       
        isInitialized = true;
    }  

    public void ShowSkid(float intencity)
    {
        if (isInitialized)
        {
           
            _lastSkidId = _skidmarkController.AddSkidMark
                (transform.position, transform.up, intencity * _intensityModifire, _lastSkidId);
            StartEmitingSmoke();
           
        }

    }

    public void StopShowingSkid()
    {
        _lastSkidId = -1;
        StopEmitingSmoke();
    }

    private void StartEmitingSmoke()
    {
        if (_smogParticles == null)
        {
            return;
        }

        if ( _smogParticles.isPlaying == false)
        {
            _smogParticles.Play();
         
        }  

    }

    private void StopEmitingSmoke()
    {
        if (_smogParticles == null)
        {
            return;
        }     
        _smogParticles.Stop();
    }
   

}
