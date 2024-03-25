using System;
using UnityEngine;

using HFSM;
using States.Base;


[RequireComponent(typeof(EntitySenses), typeof(EntityMovement), typeof(HealthComponent))]
public class CloneBrain : MonoBehaviour, IEntityBrain
{
    [SerializeField, ReadOnly] private string _currentStatePath;
    private StateMachine _rootFSM;
    private bool _processLogic = true;


    private EntitySenses _entitySenses;
    private EntityMovement _movementScript;
    private HealthComponent _healthComponent;

    private Action OnDied;


    [Header("Root States")]
    [SerializeField] private Idle _idleState;
    [SerializeField] private Chase _chaseState;
    [SerializeField] private Dead _deadState;


    private void Awake()
    {
        _entitySenses = GetComponent<EntitySenses>();
        _movementScript = GetComponent<EntityMovement>();
        _healthComponent = GetComponent<HealthComponent>();


        #region Setup FSM
        // Create the Root FSM.
        _rootFSM = new StateMachine();


        // Initialise States that need Initialisation.
        _chaseState.InitialiseValues(_movementScript, () => _entitySenses.CurrentTarget.position);


        // Add states to the Root FSM.
        _rootFSM.AddState(_idleState);
        _rootFSM.AddState(_chaseState);
        _rootFSM.AddState(_deadState);


        #region Create Transitions
        // Any > Dead
        _rootFSM.AddAnyTriggerTransition(
            to: _deadState,
            trigger: ref OnDied);

        // Investigate > Change & Vice Versa
        _rootFSM.AddTwoWayTransition(
            from: _idleState,
            to: _chaseState,
            condition: t => _entitySenses.CurrentTarget != null);
        #endregion


        // Set Initial State.
        _rootFSM.SetStartState(_idleState);

        // Initialise the Root FSM
        _rootFSM.Init();
        #endregion
    }

    #region Event Subscription
    private void OnEnable()
    {
        _healthComponent.OnDeath.AddListener(Dead);

        GameManager.OnHaultLogic += () => _processLogic = false;
        GameManager.OnResumeLogic += () => _processLogic = true;
    }
    private void OnDisable()
    {
        _healthComponent.OnDeath.RemoveListener(Dead);

        GameManager.OnHaultLogic -= () => _processLogic = false;
        GameManager.OnResumeLogic -= () => _processLogic = true;
    }
    #endregion


    private void Update()
    {
        if (!_processLogic)
            return;

        _rootFSM.OnTick();


        // Debug Stuff.
        _currentStatePath = _rootFSM.GetActiveHierarchyPath();
    }
    private void FixedUpdate()
    {
        if (!_processLogic)
            return;

        _rootFSM.OnFixedTick();
    }

    private void Dead() => OnDied?.Invoke();
}
