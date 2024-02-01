using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HFSM;
using States.Base;


public class TestEnemy : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _currentStatePath;


    [Space(10)]
    [SerializeField] private Position _playerPosition;
    private Vector2? _currentTargetPosition
    {
        get
        {
            if (_canSeePlayer == false)
                return null;

            return _playerPosition.Value;
        }
    }


    [Header("Root States")]
    [SerializeField] private Unaware _unawareFSM;
    [SerializeField] private Combat _combatFSM;
    [SerializeField] private Dead _deadState;


    [Header("Unaware States")]
    [SerializeField] private Idle _idleState;
    [SerializeField] private Patrol _patrolState;


    [Header("Combat States")]
    [SerializeField] private Chase _chaseState;


    [Header("Testing")]
    [SerializeField] private bool _isDead;
    [SerializeField] private bool _canSeePlayer;

    [Space(5)]
    [SerializeField] private bool _returnToIdle;



    private StateMachine _rootFSM;
    private void Awake()
    {
        #region State Machine Setup
        _rootFSM = new StateMachine();

        // State Initialisation.
        _patrolState.InitialiseValues(this.transform);
        _chaseState.InitialiseValues(() => _currentTargetPosition.Value);


        #region Root FSM Setup
        _rootFSM.AddState(_unawareFSM);
        _rootFSM.AddState(_combatFSM);
        _rootFSM.AddState(_deadState);

        // Initial State.
        _rootFSM.SetStartState(_unawareFSM);

        #region Transitions
        _rootFSM.AddAnyTransition(
            to: _deadState,
            condition: t => _isDead);

        _rootFSM.AddTwoWayTransition(
            from: _unawareFSM,
            to: _combatFSM,
            condition: t => _canSeePlayer);
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
        _unawareFSM.AddAnyTransition(
            to: _idleState,
            condition: t => _returnToIdle);
        #endregion
        #endregion


        #region Combat FSM Setup
        _combatFSM.AddState(_chaseState);

        // Initial State
        _combatFSM.SetStartState(_chaseState);

        #region Transitions
        #endregion
        #endregion


        // Initialise the Root FSM
        _rootFSM.Init();
        #endregion
    }

    private void Update()
    {
        // Notify the Root State Machine to run OnLogic.
        _rootFSM.OnTick();


        // Test Variable Resetting.
        if (_returnToIdle)
            _returnToIdle = false;


        // Debug Stuff.
        _currentStatePath = _rootFSM.GetActiveHierarchyPath();
    }
    private void FixedUpdate() => _rootFSM.OnFixedTick(); // Notify the Root State Machine to run OnFixedLogic.
}
