using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BonusView : MonoBehaviour
{
    public event Action ViewIsActiveEvent;

    [SerializeField] private MeshRenderer[] _meshs;
    [SerializeField] private TMP_Text _value;

    private BoxCollider _collider;
    private BonusIcon _icon;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _meshs =GetComponentsInChildren<MeshRenderer>();
        _value=GetComponentInChildren<TMP_Text>();
        _icon = GetComponentInChildren<BonusIcon>();
    }

    private void OnEnable()
    {
        SetActive(true);
        
    }

    public void SetActive(bool isActive)
    {
        foreach(var mesh in _meshs)
        {
            mesh.enabled= isActive;
        }      
        _collider.enabled = isActive;
        _value.enabled = isActive;

        if (isActive)
        {
            ViewIsActiveEvent?.Invoke();
            _icon.StartRotation(3f);
        }
        else
        {
            _icon.StopRotation();
        }
    }    

    public void SetValue(int value)
    {
        _value.text = value.ToString();      
    }
   
}
