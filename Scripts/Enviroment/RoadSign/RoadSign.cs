using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SignType
{
    None,
    Stop,
    Bonus

}

public class RoadSign : PoolObj

{
    public override bool IsActive => _isActive;

    [SerializeField] private Material _stopMaterial;
    [SerializeField] private Material _repearMaterial;


    private PoolObj _owner;
    private bool _isActive;   
    private MeshRenderer _renderer;  


    private void Awake()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
       
        SetMarkType(SignType.None);
    }

    private void OnEnable()
    {
        SetActive(true);
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }


    public void SubscribeToOwner(PoolObj owner)
    {
        _owner = owner;
        _owner.ObjIsActiveEvent += OnOwnerIsActive;
    } 
    
    public void  SetMarkType(SignType markType) {

        switch(markType)
        {
            case SignType.None:

                _renderer.material = null;
                break;

            case SignType.Stop:
                _renderer.material = _stopMaterial;
                break;

            case SignType.Bonus:
                _renderer.material = _repearMaterial;
                break;

        }
    
    }

    private void OnOwnerIsActive(bool isActive)
    {
        SetActive(isActive);
    }

  

    public override void SetActive(bool isActive)
    {
        _renderer.enabled = isActive;
        _isActive = isActive;
    }

    private void UnSubscribe()
    {
        if(_owner!= null)
        {
            _owner.ObjIsActiveEvent -= OnOwnerIsActive;
        }
    }


}
