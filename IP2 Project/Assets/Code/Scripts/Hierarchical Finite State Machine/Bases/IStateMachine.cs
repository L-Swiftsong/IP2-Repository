namespace HFSM
{
    /// <summary> A subset of features that every parent state machine must provide.</summary>
    public interface IStateMachine
    {
        /// <summary> Tells the State Machine to perform any pending state transition requests.</summary>
        void StateCanExit();

        bool HasPendingTransition { get; }
    }
}