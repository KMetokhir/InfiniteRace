using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DestructionModel 
{ 
    private Door[] _doors;
    private Hood _hood;
    private AbstractDestroyablePart[] _destoyableParts;
    
    private Transform _carBodyTr; 

    private Durability _durability;

    private ParticleSystem _smokeParticle;
    private ParticleSystem _fireParticle;
    private ParticleSystem _exploisionParticle;

    private Dictionary<ParticleSystem, float[]> _particlesRateSettingsDic = new Dictionary<ParticleSystem,  float[]>();
    private Dictionary<ParticleSystem, Sequence> _particlesStartSqequenseDic = new Dictionary<ParticleSystem, Sequence>();
    private Dictionary<ParticleSystem, Sequence> _particlesStopSqequenseDic = new Dictionary<ParticleSystem, Sequence>();


    public DestructionModel(Durability durability, Door[] doors, Hood hood, AbstractDestroyablePart[] destoyableParts, Transform carBodyTr, ParticleSystem smokeParticle,  ParticleSystem fireParticle, ParticleSystem exploisionParticle)
    {
        _durability = durability;

        _doors = doors;
        _hood = hood;
        _destoyableParts = destoyableParts;

        _carBodyTr = carBodyTr;

        _durability.ValueChangedEvent += OnDurabilityChangedEvent;

        _durability.DeadEvent += OnDeadEvent;

        _smokeParticle = smokeParticle;
        _fireParticle = fireParticle;
        _exploisionParticle = exploisionParticle;

        SetParticalrateDictionary(_smokeParticle);
        SetParticalrateDictionary(_fireParticle);
        SetParticalrateDictionary(_exploisionParticle);
    }
    

    private void OnDeadEvent()
    {
        _exploisionParticle.Play();
        foreach(var part in _destoyableParts)
        {
            part.Destroy(_carBodyTr.position, 10f, Vector3.zero);
        }     

    }


    private void SetParticalrateDictionary(ParticleSystem particleSystem)
    {
        var particles = particleSystem.GetComponentsInChildren<ParticleSystem>();

        foreach(var particle in particles)
        {

            _particlesRateSettingsDic.Add(particle, new float[2] { particle.emission.rateOverTime.constant, particle.emission.rateOverDistance.constant });

            var emissionModule = particle.emission;
            emissionModule.rateOverTime = 0f;
            emissionModule.rateOverDistance = 0f;
        }
    }

   

    private Sequence ParticleEmissionRangeSoftChange(ParticleSystem particleSystem, float targetEmissionRateByTime,  float targetEmissionRateByDistance, float timeRateDuration, float distanceRateDuration)
    {
       

        var emissionModule = particleSystem.emission;       

        float timeRate = emissionModule.rateOverTime.constant;
        float distanceRate = emissionModule.rateOverDistance.constant;


       Sequence sequence = DOTween.Sequence();

       Tweener timeTweener= DOTween.To(() => timeRate, x => timeRate = x, targetEmissionRateByTime, timeRateDuration).SetEase(Ease.InExpo).OnUpdate(() => emissionModule.rateOverTime = timeRate).Pause();
       Tweener distanceTweener = DOTween.To(() => distanceRate, x => distanceRate = x, targetEmissionRateByDistance, distanceRateDuration).SetEase(Ease.InExpo).OnUpdate(() => emissionModule.rateOverDistance = distanceRate).Pause();
      
       sequence.Join(timeTweener).Join(distanceTweener);     
       sequence.Play();

       return sequence;   
    }

  

    private void PartIcleSystemSoftStart(ParticleSystem particleSystem , float timeRateDuration, float distanceRateDuration, Dictionary<ParticleSystem, float[]> particlesRateSettingsDic)
    {       

        if (_particlesStopSqequenseDic.ContainsKey(particleSystem))
        {
          
            if (_particlesStopSqequenseDic[particleSystem].IsPlaying())
            {           
                _particlesStopSqequenseDic[particleSystem].Kill();   
            }
        }

        if (particleSystem.isEmitting)
        {           
            return;
        }

        ParticleSystem[] innerParticles =particleSystem.GetComponentsInChildren<ParticleSystem>();
              

        foreach (var innerParticle in innerParticles)
        {
            if (_particlesStopSqequenseDic.ContainsKey(innerParticle))               
                 _particlesStopSqequenseDic[innerParticle].Kill();           

           float timeRate =  particlesRateSettingsDic[innerParticle][0];
           float distanceRate = particlesRateSettingsDic[innerParticle][1];         

           Sequence sequence =  ParticleEmissionRangeSoftChange(innerParticle, timeRate, distanceRate, timeRateDuration,  distanceRateDuration);

            
            if (_particlesStartSqequenseDic.ContainsKey(innerParticle))
            {
                _particlesStartSqequenseDic[innerParticle] = sequence;

            }
            else
            {
                
                _particlesStartSqequenseDic.Add(innerParticle, sequence);
            }


        }

        particleSystem.Play();
    }

  


    private void PartIcleSystemSoftStop(ParticleSystem particleSystem, float timeRateDuration, float distanceRateDuration)
    {
      

        if (_particlesStopSqequenseDic.ContainsKey(particleSystem))
            if (_particlesStopSqequenseDic[particleSystem].IsPlaying())
                return;

        if (particleSystem.isStopped)
            return;


        ParticleSystem[] innerParticles = particleSystem.GetComponentsInChildren<ParticleSystem>();
    

        foreach (var innerParticle in innerParticles)
        {
            if (_particlesStartSqequenseDic.ContainsKey(innerParticle))
                _particlesStartSqequenseDic[innerParticle].Kill();

            Sequence sequence = ParticleEmissionRangeSoftChange(innerParticle,  0f,0f, timeRateDuration, distanceRateDuration);

            if (_particlesStopSqequenseDic.ContainsKey(innerParticle))
            {
                _particlesStopSqequenseDic[innerParticle] = sequence;

            }
            else
            {
               
                _particlesStopSqequenseDic.Add(innerParticle, sequence);
            }
           
            sequence.OnComplete(() => particleSystem.Stop());         
            sequence.OnKill(() => { particleSystem.Stop(); });
        }          

    }


    private void OnDurabilityChangedEvent(int value)
    {        

        if(value < 20){

            PartIcleSystemSoftStart(_smokeParticle, 2f,2f, _particlesRateSettingsDic);
            OpenParts(_hood);
        }
        else
        {
            CloseParts(_hood);
            PartIcleSystemSoftStop(_smokeParticle, 2f, 2f);
        }


        if (value < 15)
        {
            OpenParts(_doors);
            DestroyParts(_carBodyTr.position, _carBodyTr.forward, 10f, _hood);
            PartIcleSystemSoftStart(_fireParticle, 2f, 2f, _particlesRateSettingsDic);           
        }
        else
        {
            PartIcleSystemSoftStop(_fireParticle, 2f, 2f);
            CloseParts(_doors);
            FixParts(_hood);          
        }       
        

        if (value < 10)
        {
            DestroyParts( _carBodyTr.position, _carBodyTr.forward, 7f, _doors);
        }
        else
        {
           FixParts(_doors);
        }

    }

    private void OpenParts( params OpenablePart[] parts)
    {
        foreach (var part in parts)
        {
            part.Open();
        }
    }

    private void DestroyParts( Vector3 direction, Vector3 directionOffset, float forceMagnitude, params AbstractDestroyablePart[] parts) {

        foreach (var part in parts)
        {
            part.Destroy(direction, forceMagnitude, directionOffset);
        }
    }

    private void CloseParts(params OpenablePart[] parts)
    {
        foreach (var part in parts)
        {
            part.Close();
        }
    }

    private void FixParts(params AbstractDestroyablePart[] parts)
    {
        foreach (var part in parts)
        {
            part.Fix();
        }
    }

}
