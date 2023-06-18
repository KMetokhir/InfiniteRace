using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BlockView : MonoBehaviour
{
    public event Action ViewIsActiveEvent;

    [SerializeField] private TMP_Text _durationLevel;
    [SerializeField] private ParticleSystem _destroyParticles;

    private BoxCollider _collider;
    private float _destroyparticleStartMaxSpeed = 45f;
    private float _destroySpeedOffset = 5f;
    private Wall _blockVisual;

   

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _durationLevel = GetComponentInChildren<TMP_Text>();    
        _destroyParticles = GetComponentInChildren<ParticleSystem>();
        _blockVisual = GetComponentInChildren<Wall>();    
    }

    private void OnEnable()
    {

        SetActive(true);

    }


    public void SetDurationLevel(int value, string mess = "null" )
    {
        _durationLevel.text = value.ToString();  
    }

    public void SetActive(bool isActive)
    {
       
        _collider.enabled = isActive;
        _blockVisual.gameObject.SetActive(isActive);
        _durationLevel.enabled = isActive;

        if (isActive)
        {
            ViewIsActiveEvent?.Invoke();
        }

    }

    public void Destroy(float carVelocity, float carMaxVelocity)
    {
        _destroyparticleStartMaxSpeed = carMaxVelocity + _destroySpeedOffset;

        var multiplaier = carVelocity / carMaxVelocity;

        var main = _destroyParticles.main;        
        main.startSpeed = _destroyparticleStartMaxSpeed* multiplaier;

        _destroyParticles.Play();       

        SetActive(false);

      
    }

}
