using System;
namespace HFSM
{
    public class TriggerTransition : TransitionBase
    {
        private bool _shouldTransition;

        private Func<TriggerTransition, bool> _condition;
        private Action<TriggerTransition> _beforeTransition;
        private Action<TriggerTransition> _afterTransition;


        /// <summary> Initialises a new instance of the TransitionOnTrigger class.</summary>
        /// <param name="trigger"> The trigger that must have been triggered for this transition to be valid.</param>
        /// <param name="onTransition"> A callback function that is called just before the transition.</param>
        /// <param name="afterTransition"> A callback function that is called just after the transition.</param>
        /// <inheritdoc cref="TransitionBase(IState, IState, bool)"/>
        public TriggerTransition(
            IState from,
            IState to,
            ref Action trigger,
            Func<TriggerTransition, bool> condition = null,
            Action<TriggerTransition> onTransition = null,
            Action<TriggerTransition> afterTransition = null,
            bool forceInstantly = false) : base(from, to, forceInstantly)
        {
            trigger += () => _shouldTransition = true;
            this._shouldTransition = false;
            
            this._condition = condition;
            this._beforeTransition = onTransition;
            this._afterTransition = afterTransition;
        }


        public override void OnEnter() => _shouldTransition = false;


        public override bool ShouldTransition()
        {
            // If the event has not been triggered, then we should not transition.
            if (_shouldTransition == false)
                return false;

            // If this transition has no condition, then we should transition.
            if (_condition == null)
                return true;

            // Otherwise return the returned value of the condition.
            return _condition(this);
        }


        public override void BeforeTransition() => _beforeTransition?.Invoke(this);
        public override void AfterTransition()
        {
            _shouldTransition = false;
            _afterTransition?.Invoke(this);
        }
    }
}