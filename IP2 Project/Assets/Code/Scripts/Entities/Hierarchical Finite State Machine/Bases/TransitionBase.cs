namespace HFSM
{
    public class TransitionBase : ITransitionListener
    {
        public IState From;
        public IState To;

        public bool ForceInstantly;
        public bool IsExitTransition;

        public IStateMachine FSM;


        /// <summary> Initialises a new instance of the TransitionBase class.</summary>
        /// <param name="from"> The name/identifier of the active state.</param>
        /// <param name="to"> The name/identifier of the next state.</param>
        /// <param name="forceInstantly"> Determines if we should ignore the NeedsExitTime of the active state to force an instant transition.</param>
        public TransitionBase(IState from, IState to, bool forceInstantly = false)
        {
            this.From = from;
            this.To = to;
            this.ForceInstantly = forceInstantly;
        }


        /// <summary> Called to initialise the transition, after values like 'FSM' have been set.</summary>
        public virtual void Init()
        {

        }

        /// <summary> Called when the State Machine enters the "From" state.</summary>
        public virtual void OnEnter()
        {

        }
        /// <summary> Called when the State Machine exits the "From" state.</summary>
        public virtual void OnExit()
        {

        }


        /// <summary> Called to determine whether the state machine should transition to the "To" state..</summary>
        /// <returns> True if the state machine should transition.</returns>
        public virtual bool ShouldTransition()
        {
            return true;
        }


        /// <summary> A callback method that is called just before the transition occurs.</summary>
        public virtual void BeforeTransition()
        {

        }
        /// <summary> A callback method that is called just after the transition occurs.</summary>
        public virtual void AfterTransition()
        {

        }
    }
}