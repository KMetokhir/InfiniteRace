using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BonusIcon : MonoBehaviour
{
    private Tween _rotateTween;
    private Quaternion _defaultRotationAngle;

    private void Awake()
    {      
        _defaultRotationAngle = transform.rotation;
    }

    public void StartRotation(float rotationTime)
    {
        if (_rotateTween.IsActive())
        {
            return;
        }

        _rotateTween = transform.DORotate(new Vector3(90,180,0), rotationTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
       
    }

    public void StopRotation()
    {
        if (_rotateTween.IsActive())
        {
            _rotateTween.Kill();
        }

        transform.rotation = _defaultRotationAngle;
    }

}



