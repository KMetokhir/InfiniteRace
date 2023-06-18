using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OpenablePart : AbstractDestroyablePart
{
    private OpenablePartModel _model;   

    private bool _isInitializedOpenPart = false;
  

    protected  void InitializeOpenablePart(OpenablePartModel partModel, AbstractCarPartView partView, AbstractDestroyedCarPartView destroyedPartV)
    {
        if (_isInitializedOpenPart)
        {
            Debug.LogError("OpenablePart class was Initialized allredy");
            return;
        }

        base.InitializeDestroyblePart(partModel, partView, destroyedPartV);
        _model = partModel;         

       _isInitializedOpenPart = true;
    }

    protected new  void InitializeDestroyblePart(AbstractDestroyablePartModel partModel, AbstractCarPartView partView, AbstractDestroyedCarPartView destroyedPartV) 
    {
        Debug.LogError("You can'not Initialize Destroyble class from openable class directly");
    }

    private void OnEnable()
    {

        if (!_isInitializedOpenPart)
        {
            Debug.LogError("OpenablePart class  Does not Initialized in Child Awake");
            return;
        }

    }

    public void Open()
    {
        if (!_isInitializedOpenPart)
        {            
            return;
        }
        _model.Open();
    }
     

    public void Close()
    {
        if (!_isInitializedOpenPart)
        {
           return;
        }
        _model.Close();
    } 

    
}
