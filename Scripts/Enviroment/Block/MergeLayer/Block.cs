using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : PoolObj, IDamageable 
{
    public override bool IsActive { get { return _isActive; } }

    [SerializeField] private Vector2Int _destroyPriceRange;
    [SerializeField] private BlockView _view;

  
    private BlockModel _model;
    private float _carHitVelocity;
    private float _carMaxVelocity;
    private bool _isActive;
   

    private void Awake()
    {     
        _view = this.gameObject.AddComponent<BlockView>();
        _model = new BlockModel(_destroyPriceRange);
     
        _model.DurabilityChangedEvent += OnDurabilityChanged;        
    }

    private void OnEnable()
    {
        _view.ViewIsActiveEvent += OnViewActive;
        _model.DeadEvent += OnDead;

        SetActive(true);
    }

    private void OnDisable()
    {
        _view.ViewIsActiveEvent -= OnViewActive;
        _model.DeadEvent -= OnDead;

        _model.DurabilityChangedEvent -= OnDurabilityChanged;
    }

    public int TakeDamage(int value, float carVelocity, float carMaxVelocity)
    {
        _carHitVelocity = carVelocity;
        _carMaxVelocity = carMaxVelocity;
        return _model.TakeDamage(value);
    }

    public override void SetActive(bool isActive)
    {
        _isActive = isActive;
        _view.SetActive(isActive);

        ObjIsActiveEvent?.Invoke(isActive);
    }

    private void OnViewActive()
    {
        _model.Refresh();        

    }

    private void OnDurabilityChanged(int value)
    {
       // _view.SetDurationLevel(value);        
    }   

    private void OnDead()
    {      
       // _view.SetDurationLevel(0, "Destroy in view");      
        _view.SetActive(false); 
        _view.Destroy(_carHitVelocity, _carMaxVelocity);    
    }

   
}
