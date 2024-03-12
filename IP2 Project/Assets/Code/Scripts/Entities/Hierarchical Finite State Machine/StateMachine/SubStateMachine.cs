namespace HFSM
{
    public abstract class SubStateMachine<TEvent> : StateMachine<TEvent>, IState
    {
        public abstract string Name { get; }
        
        public bool NeedsExitTime { get; }
        public bool IsGhostState { get; }
        
        public IStateMachine FSM { get; set; }


        /// <summary> Initialises a new instance of a SubStateMachine class.</summary>
        /// <param name="needsExitTime"> Determines whether the State Machine as a state of a parent state machine is allowed to instantly exit on
        ///     a transition (False), or if it should wait until an explicit exit transition occurs (True).</param>
        /// <param name="isGhostState">If true, this state becomes a Ghost State, which the FSM doesn't want to stay in,
        ///     and will test all outgoing transitions instantly, as opposed to waiting for the next OnLogic call.</param>
        public SubStateMachine(bool needsExitTime = false, bool isGhostState = false)
        {
            this.NeedsExitTime = needsExitTime;
            this.IsGhostState = isGhostState;
        }


        /// <summary> Signals to the parent FSM that this FSM can exit, which allows the parent FSM to transition to the next state.</summary>
        private void PerformVerticalTransition()
        {
            FSM?.StateCanExit();
        }

        protected override bool TryTransition(TransitionBase transition)
        {
            if (transition.IsExitTransition)
            {
                // If the parent FSM is null, or has a pending transition, or we shouldn't/can't transition, then don't transition.
                if (FSM == null || !FSM.HasPendingTransition || !transition.ShouldTransition() || !CanExit())
                    return false;

                // Otherwise, request an exit transition.
                RequestExit(transition.ForceInstantly, transition as ITransitionListener);
                return true;
            }
            else
            {
                // If the transition says we shouldn't transition, then don't transition.
                if (!transition.ShouldTransition())
                    return false;

                // Otherwise, request a State Change.
                RequestStateChange(transition.To, transition.ForceInstantly, transition as ITransitionListener);
                return true;
            }
        }

        /// <summary> Notifies the State Machine that the state can clearnly exit, allowing us to execute any pending transitions.</summary>
        public override void StateCanExit()
        {
            // If there is no pending transition, return.
            if (!CurrentPendingTransition.IsPending)
                return;


            ITransitionListener listener = CurrentPendingTransition.Listener;

            if (CurrentPendingTransition.IsExitTransition)
            {
                // This is a vertical transition (Up the Hierarchy).
                CurrentPendingTransition = default;

                listener?.BeforeTransition();
                PerformVerticalTransition();
                listener?.AfterTransition();
            }
            else
            {
                // This is a horizontal transition (Across the Hierarchy).
                IState state = CurrentPendingTransition.TargetState;

                // When the pending state is a Ghost State, ChangeState() will have to try all outgoing transitions, which may override the pendingState.
                // That is why we are clearing the pending state beforehand, and not afterwards, as that would override the new valid pending state.
                CurrentPendingTransition = default;
                ChangeState(state, listener);
            }
        }

        /// <summary> Requests a vertical transition, allowing the state machine to allow the parent FSM to transition to the next state, respecting the NeedsExitTime of the currently active state.</summary>
        /// <param name="forceInstantly"> Overrides the NeedsExitTime of the active state if true, forcing an immediate state change.</param>
        /// <param name="listener"> An optional object that recieves callbacks before and after the transition.</param>
        public void RequestExit(bool forceInstantly = false, ITransitionListener listener = null)
        {
            // If we cannot currently exit, then stop the exit request.
            if (!CanExit())
                return;
            
            // If we don't need to wait for the state to exit, then instantly transition.
            if (!ActiveState.NeedsExitTime || forceInstantly)
            {
                CurrentPendingTransition = default;
                listener?.BeforeTransition();
                PerformVerticalTransition();
                listener?.AfterTransition();
            }
            else
            {
                CurrentPendingTransition = PendingTransition.CreateForExit(listener);
                ActiveState.OnExitRequest();
            }
        }



        // Nothing should be called on Init for a SubStateMachine (Or at least none of the base implementation).
        public override void Init() { }
        public virtual void OnEnter() => base.Init();
        public virtual void OnLogic()
        {
            // OnTick is only called externally for RootStateMachines. For all others, we call it in with OnLogic.
            OnTick();
        }
        public virtual void OnFixedLogic()
        {
            // OnFixedTick is only called externally for RootStateMachines. For all others, we call it with OnFixedLogic.
            OnFixedTick();
        }
        public virtual void OnExit() => ExitActiveState();
        
        public virtual void OnExitRequest()
        {
            UnityEngine.Debug.Log("Exit Request");
            if (CanExit() == false)
                return;

            UnityEngine.Debug.Log("Can Exit Passed");
            
            if (ActiveState.NeedsExitTime)
                ActiveState.OnExitRequest();
            else
                FSM.StateCanExit();
        }
        public virtual bool CanExit() => true;


        public override string GetActiveHierarchyPath()
        {
            if (ActiveState == null)
            {
                // When the state machine is not active, then the active hierarchy path is empty.
                return "";
            }

            return $"{Name}/{ActiveState.GetActiveHierarchyPath()}";
        }
    }

    #region Overloaded Classes
    public abstract class SubStateMachine : SubStateMachine<System.Action>
    {
        
    }
    #endregion
}