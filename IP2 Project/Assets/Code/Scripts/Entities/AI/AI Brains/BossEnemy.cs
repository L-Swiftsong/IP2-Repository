using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HFSM;
using States.Base;
using States.Alternative;

public class BossEnemy : MonoBehaviour, IEntityBrain
{
    [SerializeField, ReadOnly] private string _currentStatePath;
    private StateMachine _rootFSM;
    private bool _processLogic = true;

    [SerializeField, ReadOnly] private Vector2? _investigatePosition;


    private Action OnBattleStarted;
    private Action OnStunned;
    private Action OnDied;


    [Header("Targeting")]
    private Transform _player;
    private Transform _currentTarget => _entitySenses.CurrentTarget != null ? _entitySenses.CurrentTarget : _player;


    [Header("Root States")]
    [SerializeField] private Idle _idleState;
    [SerializeField] private HealthBasedAttacking _attackingState;
    [SerializeField] private Stunned _stunnedState;
    [SerializeField] private Dead _deadState;


    [Header("References")]
    [SerializeField] private Transform _rotationPivot;
    private EntitySenses _entitySenses;
    private EntityMovement _movementScript;
    private HealthComponent _healthComponent;


    private void Awake()
    {
        // Targeting.
        if (PlayerManager.IsInitialised)
            _player = PlayerManager.Instance.Player.transform;

        // References.
        _entitySenses = GetComponent<EntitySenses>();
        _movementScript = GetComponent<EntityMovement>();
        _healthComponent = GetComponent<HealthComponent>();

        #region Setup FSM
        // Create the Root FSM.
        _rootFSM = new StateMachine();


        // Initialise States that need Initialisation.
        _attackingState.InitialiseValues(this, _rotationPivot, _movementScript, _healthComponent, () => _currentTarget.position);


        #region Root FSM Setup
        // Add States.
        _rootFSM.AddState(_idleState);
        _rootFSM.AddState(_attackingState);
        _rootFSM.AddState(_stunnedState);
        _rootFSM.AddState(_deadState);

        // Set Initial State.
        _rootFSM.SetStartState(_idleState);

        #region Create Transitions
        // Any > Dead
        _rootFSM.AddAnyTriggerTransition(
            to: _deadState,
            trigger: ref OnDied);


        // Idle > Attacking
        _rootFSM.AddTriggerTransition(
            from: _idleState,
            to: _attackingState,
            trigger: ref OnBattleStarted);


        // Stunned Transitions.
        _rootFSM.AddTriggerTransition(
            from: _attackingState,
            to: _stunnedState,
            trigger: ref OnStunned);

        _rootFSM.AddTransition(
            from: _stunnedState,
            to: _attackingState,
            condition: t => _stunnedState.HasStunCompleted);
        #endregion
        #endregion


        // Initialise the Root FSM
        _rootFSM.Init();
        #endregion
    }

    private void Start()
    {
        // ----- For Testing. Remove once we've confirmed it works. -----
        OnBattleStarted?.Invoke();
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

        GameManager.OnHaultLogic -= () => _processLogic = false;
        GameManager.OnResumeLogic -= () => _processLogic = true;
    }
    #endregion


    private void Update()
    {
#if UNITY_EDITOR
        if (_player == null && PlayerManager.IsInitialised)
                _player = PlayerManager.Instance.Player.transform;
#endif

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

        _rootFSM.OnFixedTick(); // Notify the Root State Machine to run OnFixedLogic.
    }


    private void DamageTaken(HealthChangedValues changedValues)
    {
        // Calculate the damage taken.
        float damageTaken = changedValues.OldHealth - changedValues.NewHealth;

        // If the damage taken is greater than or equal to the stunned state's stun threshold, and we don't resist it, then trigger the stun.
        if (damageTaken >= _stunnedState.StunThreshold && _stunnedState.HasResistedStun() == false)
            OnStunned?.Invoke();
    }
    private void Dead() => OnDied?.Invoke();
}
