using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : PoolObj, IBonus 
{
    [SerializeField] private Vector2Int _valueRange;
    public override bool IsActive { get { return _isActive; } }

    private BonusModel _model;
    private BonusView _view;
    private bool _isActive;
    

    private void Awake()
    {        
        _view = gameObject.AddComponent<BonusView>();
        _model = new BonusModel(_valueRange);
    }

    private void OnEnable()
    {
        _view.ViewIsActiveEvent += OnViewEnable;

        _model.CollectedEvent += OnCollected;
        _model.ValueSetEvent += OnValueSet;

        SetActive(true);

    }

    private void OnDisable()
    {
        _view.ViewIsActiveEvent -= OnViewEnable;

        _model.CollectedEvent -= OnCollected;
        _model.ValueSetEvent -= OnValueSet;
    }

    private void OnViewEnable()
    {
        _model.Enable();
    }

    private void OnValueSet(int value)
    {
       // _view.SetValue(value);
    }

    private void OnCollected()
    {
        _view.SetActive(false);
    }

    public int Collect()
    {
        _view.SetActive(false);
        return _model.Collect();
    }

    public override void SetActive(bool isActive)
    {
        _view.SetActive(isActive);
        _isActive = isActive;
        ObjIsActiveEvent?.Invoke( isActive);
    }
}
