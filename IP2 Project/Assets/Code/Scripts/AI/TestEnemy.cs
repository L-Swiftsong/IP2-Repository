using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HFSM;
using States.Base;


public class TestEnemy : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _currentStatePath;


    [Space(10)]
    [SerializeField] private EntitySenses _entitySenses;
    [SerializeField] private HealthComponent _healthComponent;

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
    [SerializeField] private bool _drawPatrolGizmos;
    [SerializeField] private bool _drawAttackingGizmos;



    private StateMachine _rootFSM;
    private void Awake()
    {
        if (_healthComponent == null)
            _healthComponent = GetComponent<HealthComponent>();

        
        #region State Machine Setup
        _rootFSM = new StateMachine();

        // State Initialisation.
        _patrolState.InitialiseValues(this.transform);
        _chaseState.InitialiseValues(() => _entitySenses.CurrentTarget.position, this.transform);
        _attackingState.InitialiseValues(() => _entitySenses.CurrentTarget.position, this.transform);


        #region Root FSM Setup
        _rootFSM.AddState(_unawareFSM);
        _rootFSM.AddState(_combatFSM);
        _rootFSM.AddState(_stunnedState);
        _rootFSM.AddState(_deadState);

        // Initial State.
        _rootFSM.SetStartState(_unawareFSM);

        #region Transitions
        _rootFSM.AddAnyTriggerTransition(
            to: _deadState,
            trigger: ref OnDied);

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
        _unawareFSM.AddState(_idleState);
        _unawareFSM.AddState(_patrolState);

        // Initial State.
        _unawareFSM.SetStartState(_idleState);

        #region Transitions
        _unawareFSM.AddTransition(
            from: _idleState,
            to: _patrolState,
            condition: t => _idleState.IsDelayComplete);
        #endregion
        #endregion


        #region Combat FSM Setup
        _combatFSM.AddState(_chaseState);
        _combatFSM.AddState(_attackingState);

        // Initial State
        _combatFSM.SetStartState(_chaseState);

        #region Transitions
        _combatFSM.AddTwoWayTransition(
            from: _chaseState,
            to: _attackingState,
            condition: t => Vector2.Distance(transform.position, _entitySenses.CurrentTarget.position) <= _attackingState.MaxDistance);
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
    


    private void OnDrawGizmos()
    {
        if (_drawPatrolGizmos && _patrolState != null)
            _patrolState.DrawGizmos();

        if (_drawAttackingGizmos && _attackingState != null)
            _attackingState.DrawGizmos(transform);
    }
}
