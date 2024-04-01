using System;
using UnityEngine;

using HFSM;
using States.Base;


[RequireComponent(typeof(EntitySenses), typeof(EntityMovement), typeof(HealthComponent))]
public class StandardEnemy : MonoBehaviour, IEntityBrain
{
    [SerializeField, ReadOnly] private string _currentStatePath;
    private StateMachine _rootFSM;
    private bool _processLogic = true;

    [SerializeField, ReadOnly] private Vector2? _investigatePosition;


    [Space(5)]
    [SerializeField] private Transform _rotationPivot;
    private EntitySenses _entitySenses;
    private EntityMovement _movementScript;
    private HealthComponent _healthComponent;
    private AbilityHolder abilityHolder;

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
    [SerializeField] private Investigate _investigateState;


    [Header("Combat States")]
    [SerializeField] private Chase _chaseState;
    [SerializeField] private Attacking _attackingState;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private bool _drawCBSBehaviourGizmos;

    [SerializeField] private bool _drawInvestigatePosition;

    [Space(5)]
    [SerializeField] private bool _drawAttackingStateGizmos;




    public void Awake()
    {
        _entitySenses = GetComponent<EntitySenses>();
        _movementScript = GetComponent<EntityMovement>();
        _healthComponent = GetComponent<HealthComponent>();
        abilityHolder = gameObject.GetComponent<AbilityHolder>();

        #region Setup FSM
        // Create the Root FSM.
        _rootFSM = new StateMachine();


        // Initialise States that need Initialisation.
        _patrolState.InitialiseValues(_movementScript);
        _investigateState.InitialiseValues(_movementScript, () => _investigatePosition.Value);

        _chaseState.InitialiseValues(_movementScript, () => _entitySenses.CurrentTarget.position);
        _attackingState.InitialiseValues(this, _rotationPivot, _movementScript, () => _entitySenses.CurrentTarget.position);


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
        _unawareFSM.AddState(_investigateState);

        // Set Initial State.
        _unawareFSM.SetStartState(_idleState);

        #region Create Transitions
        // Idle > Patrol.
        _unawareFSM.AddTransition(
            from: _idleState,
            to: _patrolState,
            condition: t => _idleState.IsDelayComplete);


        // Idle > Investigate.
        _unawareFSM.AddTransition(
            from: _idleState,
            to: _investigateState,
            condition: t => _investigatePosition.HasValue);

        // Investigate > Idle (Set investigate position to null when we do this transition).
        _unawareFSM.AddTransition(
            from: _investigateState,
            to: _idleState,
            condition: t => _investigateState.WithinInvestigationRadius(),
            onTransition: t => _investigatePosition = null);
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

        GameManager.OnHaultLogic -= () => _processLogic = false;
        GameManager.OnResumeLogic -= () => _processLogic = true;
    }
    #endregion


    private void Update()
    {
        if (!_processLogic)
            return;
        
        // Notify the Root State Machine to run OnLogic.
        _rootFSM.OnTick();


        // Set the current target position if we have a target.
        if (_entitySenses.CurrentTarget != null)
            _investigatePosition = _entitySenses.CurrentTarget.position;


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


    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        if (_drawInvestigatePosition && _investigatePosition.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_investigatePosition.Value, 0.25f);
        }


        // State Gizmos.
        if (_drawAttackingStateGizmos)
            _attackingState.DrawGizmos(this.transform, drawBehaviours: _drawCBSBehaviourGizmos);
    }
}
