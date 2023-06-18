using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class AbstractDestroyablePart : MonoBehaviour
{
    private AbstractDestroyablePartModel _model;  
    private AbstractCarPartView _view;
    private AbstractDestroyedCarPartView _destroyedView;

    private bool _isInitialized = false;    
   

    protected  void InitializeDestroyblePart(AbstractDestroyablePartModel partModel, AbstractCarPartView partView, AbstractDestroyedCarPartView destroyedPartV)
    {
        if (_isInitialized)
        {
            Debug.LogError("AbstractDestroyablePart class was Initialized allredy");
            return;
        }
        _model = partModel;
        _view = partView;
        _destroyedView = destroyedPartV;

        _isInitialized = true;

        _model.PartFixedEvent += PartFixed;
        _model.PartDestroiedEvent += PartDestroied;
    }


    private void OnEnable()
    {
       
        if (!_isInitialized)
        {
            Debug.LogError("AbstractDestroyablePart class  Does not Initialized in Child Awake");
            return;
        }      
       
    }

    protected virtual void Disable()
    {
        _model.PartFixedEvent -= PartFixed;
        _model.PartDestroiedEvent-= PartDestroied;       
    }

    private void OnDisable()
    {
        Disable();
    }    

    public void Destroy(Vector3 carCentrPoint, float forceMagnitude, Vector3 directionOffset)
    {
        if (!_isInitialized)
        {
            return;
        }
        _model.Destroy(carCentrPoint, forceMagnitude, directionOffset);
    }

    public void Fix()
    {
        if (!_isInitialized)
        {
            return;
        }
        _model.Fix();
    }

    private void PartFixed()
    {
        if (!_isInitialized)
        {
            return;
        }      
        _view.SetEnable(true);
        _destroyedView.SetEnable(false);
    }

    private void PartDestroied()
    {
        if (!_isInitialized)
        {
            return;
        }
        _view.SetEnable(false);
        _destroyedView.SetEnable(true);
    }
}
