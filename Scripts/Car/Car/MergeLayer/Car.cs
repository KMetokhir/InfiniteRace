using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{   
    [SerializeField] private Transform _centrOfMass;
    [SerializeField] private CarSettings _carSettings;
    [SerializeField] private ParticleSystem _smokeParticle;
    [SerializeField] private ParticleSystem _fireParticle;
    [SerializeField] private ParticleSystem _exploisionParticle;

    private CarModel _model;
    private CarView _view;
    private DynamicChassis _dynamicChassis;

    private CarInput _input;
    private Rigidbody _rigidbody;
    private GroundChecker _groundChecker;
    private CarCollisionTracker _collisionTracker;
    private Durability _durability;   

    private DestructionModel _dsModel;


    private void Awake()
    {
        _dynamicChassis = GetComponentInChildren<DynamicChassis>();
        _rigidbody = _dynamicChassis.GetComponent<Rigidbody>();            

        _collisionTracker = GetComponentInChildren<CarCollisionTracker>();       
        _input = GetComponent<CarInput>();           
        _groundChecker = GetComponentInChildren<GroundChecker>();       
        _durability = new Durability(_carSettings.StartDurabilityLevel);

        _view = GetComponentInChildren<CarView>();
        _model = new CarModel(_view.transform, _centrOfMass, _rigidbody, _groundChecker, _carSettings);

        Body body = GetComponentInChildren<Body>();   
        Door[] doors = GetComponentsInChildren<Door>();
        Hood hood = GetComponentInChildren<Hood>();
        AbstractDestroyablePart[] parts = FindObjectsOfType<AbstractDestroyablePart>();

        _dsModel = new DestructionModel(_durability, doors, hood, parts, body.transform, _smokeParticle, _fireParticle,_exploisionParticle);

    }  
   

    private void OnEnable()
    {
        _model.SlidingEvent += OnSliding;
        _model.FallOnGroundEvent += OnFallOnGround;
        _model.FrontWheelTurnEvent += OnFrontWheelTurn;   

        _collisionTracker.DamagiableCollidedEvent += OnDamagiableCollided;
        _collisionTracker.BonusCollidedEvent += OnBonusCollided;

        _view.ViewEnableEvent += OnViewEnable;
        _view.ViewDisableEvent += OnViewDisable;       

        _input.PointerTapEvent += OnPointerTap;
        _input.PointerUpEvent += OnPointerUp;
        _input.MouseB1Down += OnMouseB1Down;

        _input.PointerUpMoveEvent += OnPointerUpMove;
        _input.PointerDownMoveEvent += OnPointerDownMove;
        _input.PointerStayEvent += OnPointerStay;

        _durability.DeadEvent += OnDead;
        _durability.ValueChangedEvent += OnValueChanged;     

    }

    private void OnDisable()
    {
        _model.SlidingEvent -= OnSliding;
        _model.FallOnGroundEvent -= OnFallOnGround;
        _model.FrontWheelTurnEvent -= OnFrontWheelTurn;

        _collisionTracker.DamagiableCollidedEvent -= OnDamagiableCollided;
        _collisionTracker.BonusCollidedEvent -= OnBonusCollided;

        _view.ViewEnableEvent -= OnViewEnable;
        _view.ViewDisableEvent -= OnViewDisable;

        _input.PointerTapEvent -= OnPointerTap;
        _input.PointerUpEvent -= OnPointerUp;
        _input.MouseB1Down -= OnMouseB1Down;

        _input.PointerUpMoveEvent -= OnPointerUpMove;
        _input.PointerDownMoveEvent -= OnPointerDownMove;
        _input.PointerStayEvent -= OnPointerStay;

        _durability.DeadEvent -= OnDead;
        _durability.ValueChangedEvent -= OnValueChanged;

    }

    #region "Pointer (Drive) methods"

    private void OnPointerTap()
    {
        _model.MoveForvard();      
    }

    private void OnPointerUp() {

        _model.Brake();    
    }
    
    private void OnMouseB1Down() {

        _model.MoveBackward();       
    }

    private void OnPointerUpMove(float magnitude) {
        
        _model.TurnLeft(magnitude);    
        _view.TurningWheels(-1f);      
    }

    private void OnPointerDownMove(float magnitude) {

        _model.TurnRight(magnitude);      
        _view.TurningWheels(1f);
    
    }

    private void OnPointerStay() {
     
        _view.TurningWheels(0f);     
    }

    #endregion

    #region "Durability change methods"

    private void OnBonusCollided(IBonus bonus)
    {
      
        _durability.IncreaseValue(bonus.Collect());
    }

    private void OnDamagiableCollided(IDamageable damagiable)
    {     
        int damage = damagiable.TakeDamage(_durability.Value, _rigidbody.velocity.magnitude, _carSettings.MaxVelocity);
        _durability.DecreaseValue(damage);

    }

    private void OnValueChanged(int value)
    {
       // _view.SetDurationLevel(value);
      //  _view.SetColor(value,_carSettings.StartDurabilityLevel);
    }

    private void OnDead()
    {
        _model.StopedByDeath();

        _input.PointerTapEvent -= OnPointerTap;
        _input.PointerUpEvent -= OnPointerUp;

        _input.MouseB1Down -= OnMouseB1Down;

        _input.PointerUpMoveEvent -= OnPointerUpMove;
        _input.PointerDownMoveEvent -= OnPointerDownMove;
        _input.PointerStayEvent -= OnPointerStay;
    }

    #endregion

    #region "View"

    private void OnViewEnable()
    {
        _model.Enable();
        _durability.Enable();
    }

    private void OnViewDisable()
    {
        _model.Disable();
    }

    #endregion

    private void OnFrontWheelTurn(float turnInputTarget)
    {
        _view.TurningWheels(turnInputTarget);
    }

    private void OnFallOnGround()
    {
        _view.EmitSparkles();
    }   

    private void OnSliding(bool isSliding, float slipAmount)
    {
        if (isSliding)
        {
            _view.ShowSkeeds(slipAmount);
        }
        else
        {
            _view.StopShowingSkeeds();
        }
    }

}
