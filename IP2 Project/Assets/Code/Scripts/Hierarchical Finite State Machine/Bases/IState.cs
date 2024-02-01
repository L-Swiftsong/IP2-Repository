namespace HFSM
{
    /// <summary> The base class of all States.</summary>
    public interface IState
    {
        /// <summary> The name of this state. Used primarily for debug information. </summary>
        public string Name { get; }

        public bool NeedsExitTime { get; }
        public bool IsGhostState { get; }

        public IStateMachine FSM { get; set; }


        /// <summary> Called to initialise the state, after values like 'FSM' have been set.</summary>
        public void Init();

        /// <summary> Called when the State Machine transitions into this state (Enters this state).</summary>
        public void OnEnter();

        /// <summary> Called while this state is active (On the Update frame).</summary>
        public void OnLogic();
        
        /// <summary> Called while this state is active (On the Fixed Update frame).</summary>
        public void OnFixedLogic();

        /// <summary> Called when the State Machine transitions from this state (Exits this state).</summary>
        public void OnExit();


        /// <summary> (Only called if NeedsExitTime is true): Called when a State Transition from this state should happen.
        ///     If it can happen, it should call FSM.StateCanExit()
        ///     If it cannot exit right not, it should call FSM.StateCanExit() later (E.g. In OnLogic()).</summary>
        public void OnExitRequest();


        /// <summary> Returns a string representation of all active states in the hierarchy (E.g. "/Move/Jump/Falling").</summary>
        public virtual string GetActiveHierarchyPath()
        {
            // return this.GetType().ToString();
            return Name;
        }
    }
}