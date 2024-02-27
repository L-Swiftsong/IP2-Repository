using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HFSM;
using States.Base;


[RequireComponent(typeof(EntitySenses), typeof(EntityMovement), typeof(HealthComponent))]
public class StandardEnemy : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _currentStatePath;
    private StateMachine _rootFSM;


    private EntitySenses _entitySenses;
    private EntityMovement _movementScript;
    private HealthComponent _healthComponent;

    private Action OnStunned;
    private Action OnDied;


    [Header("Root States")]
    [SerializeField] private Unaware _unawareFSM;
    [SerializeField] private Combat _combatFSM;
    [SerializeField] private Stunned _stunnedState;
    [SerializeField] private Dead _deadState;


    [Header("Unaware States")]
    [SerializeField] private Idle _idleState;
    [SerializeField] private Patrol _patrolState;


    [Header("Combat States")]
    [SerializeField] private Chase _chaseState;
    [SerializeField] private Attacking _attackingState;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;



    private void Awake()
    {
        _entitySenses = GetComponent<EntitySenses>();
        _movementScript = GetComponent<EntityMovement>();
        _healthComponent = GetComponent<HealthComponent>();


        #region Setup FSM
        // Create the Root FSM.
        _rootFSM = new StateMachine();


        // Initialise States that need Initialisation.
        _patrolState.InitialiseValues(_movementScript);
        _chaseState.InitialiseValues(() => _entitySenses.CurrentTarget.position, _movementScript);
        _attackingState.InitialiseValues(() => _entitySenses.CurrentTarget.position, _movementScript);


        #region Root FSM Setup
        // Add States.
        _rootFSM.AddState(_unawareFSM);
        _rootFSM.AddState(_combatFSM);
        _rootFSM.AddState(_stunnedState);
        _rootFSM.AddState(_deadState);

        // Set Initial State.
        _rootFSM.SetStartState(_unawareFSM);

        #region Create Transitions
        // Any > Dead
        _rootFSM.AddAnyTriggerTransition(
            to: _deadState,
            trigger: ref OnDied);


        // Unaware > Combat & Vice-Versa
        _rootFSM.AddTwoWayTransition(
            from: _unawareFSM,
            to: _combatFSM,
            condition: t => _entitySenses.CurrentTarget != null);


        // Stunned Transitions.
        _rootFSM.AddTriggerTransition(
            from: _unawareFSM,
            to: _stunnedState,
            trigger: ref OnStunned);
        _rootFSM.AddTriggerTransition(
            from: _combatFSM,
            to: _stunnedState,
            trigger: ref OnStunned);

        _rootFSM.AddTransition(
            from: _stunnedState,
            to: _unawareFSM,
            condition: t => _stunnedState.HasStunCompleted);
        #endregion
        #endregion


        #region Unaware FSM Setup
        // Add States.
        _unawareFSM.AddState(_idleState);
        _unawareFSM.AddState(_patrolState);

        // Set Initial State.
        _unawareFSM.SetStartState(_idleState);

        #region Create Transitions
        // Idle > Patrol.
        _unawareFSM.AddTransition(
            from: _idleState,
            to: _patrolState,
            condition: t => _idleState.IsDelayComplete);
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


    private void OnEnable()
    {
        _healthComponent.OnHealthChanged.AddListener(HealthChanged);
        _healthComponent.OnDeath.AddListener(Dead);
    }
    private void OnDisable()
    {
        _healthComponent.OnHealthChanged.RemoveListener(HealthChanged);
        _healthComponent.OnDeath.RemoveListener(Dead);
    }


    private void Update()
    {
        // Notify the Root State Machine to run OnLogic.
        _rootFSM.OnTick();


        // Debug Stuff.
        _currentStatePath = _rootFSM.GetActiveHierarchyPath();
    }
    private void FixedUpdate() => _rootFSM.OnFixedTick(); // Notify the Root State Machine to run OnFixedLogic.


    private void HealthChanged(HealthChangedValues changedValues)
    {
        // Calculate the damage taken.
        float damageTaken = changedValues.OldHealth - changedValues.NewHealth;
        Debug.Log("Stunned: " + damageTaken);

        // If the damage taken is greater than or equal to the stunned state's stun threshold, then be stunned.
        if (damageTaken >= _stunnedState.StunThreshold)
            OnStunned?.Invoke();
    }
    private void Dead() => OnDied?.Invoke();
}
