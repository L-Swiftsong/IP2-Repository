using System;

namespace HFSM
{
    /// <summary> A class used to determine whether the State Machine should transition to another state, depending on a delay and an optional condition.</summary>
    public class TransitionAfter : TransitionBase
    {
        private float _delay;
        private ITimer _timer;

        private Func<TransitionAfter, bool> _condition;

        private Action<TransitionAfter> _beforeTransition;
        private Action<TransitionAfter> _afterTransition;



        public TransitionAfter(
            IState from,
            IState to,
            float delay,
            Func<TransitionAfter, bool> condition = null,
            Action<TransitionAfter> onTransition = null,
            Action<TransitionAfter> afterTransition = null,
            bool forceInstantly = false) : base(from, to, forceInstantly)
        {
            this._delay = delay;
            this._condition = condition;
            this._beforeTransition = onTransition;
            this._afterTransition = afterTransition;

            this._timer = new Timer();
        }


        public override void OnEnter() => _timer.Reset();

        public override bool ShouldTransition()
        {
            // If the timer has not yet elapsed, then we shouldn't transition.
            if (_timer.Elapsed < _delay)
                return false;

            // If we don't have a required condition, then we should transition.
            if (_condition == null)
                return true;

            // Otherwise, test the transition.
            return _condition(this);
        }

        public override void BeforeTransition() => _beforeTransition?.Invoke(this);
        public override void AfterTransition() => _afterTransition?.Invoke(this);
    }
}