namespace HFSM
{
    /// <summary> The "normal" state class that can run code on Enter, on Logic, and On Exit, while also handling the timing of the next state transition.</summary>
    public abstract class State : IState
    {
        public abstract string Name { get; }

        public bool NeedsExitTime { get; }
        public bool IsGhostState { get; }

        public IStateMachine FSM { get; set; }


        /// <summary> Initialises a new instance of the State Class.</summary>
        /// <param name="needsExitTime"> Determines if a state is allowed to instantly exit on a transition (False),
        ///     or if the State Machine should wait for until the State is ready for a state change (True).</param>
        /// <param name="isGhostState">If true, this state becomes a Ghost State, which the FSM doesn't want to stay in,
        ///     and will test all outgoing transitions instantly, as opposed to waiting for the next OnLogic call.</param>
        public State(bool needsExitTime = false, bool isGhostState = false)
        {
            this.NeedsExitTime = needsExitTime;
            this.IsGhostState = isGhostState;
        }


        public virtual void Init() { }
        public virtual void OnEnter() { }
        public virtual void OnLogic()
        {
            // If we can exit this state, and the FSM is awaiting a transition, tell the FSM that we can transition.
            if (NeedsExitTime && FSM.HasPendingTransition && CanExit())
                FSM.StateCanExit();
        }
        public virtual void OnFixedLogic() { }
        public virtual void OnExit() { }

        public virtual void OnExitRequest()
        {
            if (CanExit())
                FSM.StateCanExit();
        }

        protected virtual bool CanExit() => true;
    }
}