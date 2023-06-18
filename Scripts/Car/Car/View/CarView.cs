using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarView : MonoBehaviour
{
    public event Action ViewEnableEvent;
    public event Action ViewDisableEvent;

    [SerializeField] private Transform _leftFrontWheel;
    [SerializeField] private Transform _rightFrontWheel;
    [SerializeField] private Transform _leftRearWheel;
    [SerializeField] private Transform _rightRearWheel;
    [SerializeField] private float _maxWheelTurnAngle = 30f;
    [SerializeField] private float _wheelTurnSpeed=10;
    [SerializeField] private float _wheelSkidIntencityModifier=1f;

    [SerializeField] private ParticleSystem _sparks;

    private TMP_Text _durationLevel;
    private Renderer _renderer;   

    private float _turnInput;

    private Wheelskid[] wheelskids;
 

    private void Awake()
    {     
        wheelskids = GetComponentsInChildren<Wheelskid>();
        var skidmarkController = FindObjectOfType<Skidmarks>();
        InitWheelSkids(skidmarkController);
        _durationLevel = GetComponentInChildren<TMP_Text>();
        _renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        ViewEnableEvent?.Invoke();        
    }

    private void OnDisable()
    {
        ViewDisableEvent?.Invoke();
    }

    public void TurningWheels(float turnInputTarget)
    {
        _turnInput = (Mathf.Lerp(_turnInput, turnInputTarget, Time.fixedDeltaTime * _wheelTurnSpeed));

        _leftFrontWheel.localRotation = Quaternion.Euler(_leftFrontWheel.localRotation.eulerAngles.x, (_turnInput * _maxWheelTurnAngle) - 180, _leftFrontWheel.localRotation.eulerAngles.z);
        _rightFrontWheel.localRotation = Quaternion.Euler(_rightFrontWheel.localRotation.eulerAngles.x, (_turnInput * _maxWheelTurnAngle), _rightFrontWheel.localRotation.eulerAngles.z);

    }

    public void SetColor(int durabilityLevel, int startDurabilityLevel)
    {
        var lerpedColor = Color.Lerp(Color.red, Color.green, Mathf.Clamp((float)durabilityLevel / startDurabilityLevel, 0, 1));
        _renderer.material.color = lerpedColor;
    }

    public void ShowSkeeds(float intencity)
    {

        foreach (var wheelskid in wheelskids)
        {
            wheelskid.ShowSkid(intencity * _wheelSkidIntencityModifier);
        }

    }

    public void StopShowingSkeeds()
    {

        foreach (var wheelskid in wheelskids)
        {
            wheelskid.StopShowingSkid();
        }

    }

    public void EmitSparkles()
    {
        _sparks.Play();

    }

    public void SetDurationLevel(int value)
    {
        _durationLevel.text = value.ToString();
    }

    private void InitWheelSkids(Skidmarks skidmarkController)
    {
        foreach(var wheelskid in wheelskids)
        {
            wheelskid.Init(skidmarkController);
        }
    }   

}
