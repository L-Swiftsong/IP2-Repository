using System;
using UnityEngine;

using HFSM;
using States.Base;


[RequireComponent(typeof(EntityMovement), typeof(HealthComponent))]
public class EternallyAwareEnemy : MonoBehaviour, IEntityBrain
{
    [SerializeField, ReadOnly] private string _currentStatePath;
    private StateMachine _rootFSM;
    private bool _processLogic = true;

    [Space(5)]
    [SerializeField] private Position _targetPosition;

    [Space(5)]
    [SerializeField] private Transform _rotationPivot;
    private EntityMovement _movementScript;
    private HealthComponent _healthComponent;

    private Action OnStunned;
    private Action OnDied;


    [Header("Root States")]
    [SerializeField] private Combat _combatFSM;
    [SerializeField] private Stunned _stunnedState;
    [SerializeField] private Dead _deadState;


    [Header("Combat States")]
    [SerializeField] private Chase _chaseState;
    [SerializeField] private Attacking _attackingState;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;



    private void Awake()
    {
        _movementScript = GetComponent<EntityMovement>();
        _healthComponent = GetComponent<HealthComponent>();

        #region Setup FSM
        // Create the Root FSM.
        _rootFSM = new StateMachine();


        // Initialise States that need Initialisation.
        _chaseState.InitialiseValues(_movementScript, () => _targetPosition.Value);
        _attackingState.InitialiseValues(this, _rotationPivot, _movementScript, () => _targetPosition.Value);


        #region Root FSM Setup
        // Add States.
        _rootFSM.AddState(_combatFSM);
        _rootFSM.AddState(_stunnedState);
        _rootFSM.AddState(_deadState);

        // Set Initial State.
        _rootFSM.SetStartState(_combatFSM);

        #region Create Transitions
        // Any > Dead
        _rootFSM.AddAnyTriggerTransition(
            to: _deadState,
            trigger: ref OnDied);


        // Stunned Transitions.
        _rootFSM.AddTriggerTransition(
            from: _combatFSM,
            to: _stunnedState,
            trigger: ref OnStunned);

        _rootFSM.AddTransition(
            from: _stunnedState,
            to: _combatFSM,
            condition: t => _stunnedState.HasStunCompleted);
        #endregion
        #endregion


        #region Combat FSM Setup
        // Add States.
        _combatFSM.AddState(_chaseState);
        _combatFSM.AddState(_attackingState);

        // Set Initial State.
        _combatFSM.SetStartState(_chaseState);

        #region Create Transitions
        // Attacking > Chase & Vice Versa.
        _combatFSM.AddTwoWayTransition(
            from: _attackingState,
            to: _chaseState,
            condition: t => _attackingState.ShouldStopAttacking());
        #endregion
        #endregion


        // Initialise the Root FSM
        _rootFSM.Init();
        #endregion
    }


    #region Event Subscription
    private void OnEnable()
    {
        _healthComponent.OnDamageTaken.AddListener(DamageTaken);
        _healthComponent.OnDeath.AddListener(Dead);

        GameManager.OnHaultLogic += () => _processLogic = false;
        GameManager.OnResumeLogic += () => _processLogic = true;
    }
    private void OnDisable()
    {
        _healthComponent.OnDamageTaken.RemoveListener(DamageTaken);
        _healthComponent.OnDeath.RemoveListener(Dead);

        GameManager.OnHaultLogic += () => _processLogic = false;
        GameManager.OnResumeLogic += () => _processLogic = true;
    }
    #endregion


    private void Update()
    {
        if (!_processLogic)
            return;
        
        // Notify the Root State Machine to run OnLogic.
        _rootFSM.OnTick();


        // Debug Stuff.
        _currentStatePath = _rootFSM.GetActiveHierarchyPath();
    }
    private void FixedUpdate()
    {
        if (!_processLogic)
            return;

        // Notify the Root State Machine to run OnFixedLogic.
        _rootFSM.OnFixedTick(); 
    }

    private void DamageTaken(HealthChangedValues changedValues)
    {
        // Calculate the damage taken.
        float damageTaken = changedValues.OldHealth - changedValues.NewHealth;

        // If the damage taken is greater than or equal to the stunned state's stun threshold, and we don't resist it, then trigger the stun.
        if (damageTaken >= _stunnedState.StunThreshold)
            OnStunned?.Invoke();
    }
    private void Dead() => OnDied?.Invoke();
}
