using System;

namespace HFSM
{
    /// <summary> A class used to determine whether the State Machine should transition to another state depending on a dynamically computed delay and an optional condition.</summary>
    public class TransitionAfterDynamic : TransitionBase
    {
        private ITimer _timer;
        private float _delay;

        private bool _onlyEvaluateDelayOnEnter;
        private Func<TransitionAfterDynamic, float> _delayCalculator;

        private Func<TransitionAfterDynamic, bool> _condition;

        private Action<TransitionAfterDynamic> _beforeTransition;
        private Action<TransitionAfterDynamic> _afterTransition;


        /// <summary> Initialises a new instance of the TransitionAfterDynamic class.</summary>
        /// <param name="delay"> A function that dynamically computes the delay time.</param>
        /// <param name="condition"> A function that returns true if the State Machine should transition to the <c>To</c> state</param>
        /// <param name="onlyEvaluateDelayOnEnter"> If true, the dunamic delay is only recalculated when the <c>From</c> state is entered.
        ///     If false, the delay is evaluated with each logic step.</param>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool}
        ///     Action{Transition},
        ///     Action{Transition},
        ///     bool)"/>
        public TransitionAfterDynamic(
            IState from,
            IState to,
            Func<TransitionAfterDynamic, float> delay,
            Func<TransitionAfterDynamic, bool> condition = null,
            bool onlyEvaluateDelayOnEnter = false,
            Action<TransitionAfterDynamic> onTransition = null,
            Action<TransitionAfterDynamic> afterTransition = null,
            bool forceInstantly = false) : base(from, to, forceInstantly)
        {
            this._delayCalculator = delay;
            this._condition = condition;
            this._onlyEvaluateDelayOnEnter = onlyEvaluateDelayOnEnter;
            this._beforeTransition = onTransition;
            this._afterTransition = afterTransition;

            this._timer = new Timer();
        }


        public override void OnEnter()
        {
            _timer.Reset();

            if (_onlyEvaluateDelayOnEnter)
                _delay = _delayCalculator(this);
        }

        public override bool ShouldTransition()
        {
            // Get the current evaluation of the delay from the delay calculator if we don't only calculate it on Enter.
            if (!_onlyEvaluateDelayOnEnter)
                _delay = _delayCalculator(this);

            // If the elapsed time is not yet greater than the required delay, don't transition.
            if (_timer.Elapsed < _delay)
                return false;

            // If we don't have a condition, then we should transition.
            if (_condition == null)
                return false;

            // Otherwise, test the condition.
            return _condition(this);
        }


        public override void BeforeTransition() => _beforeTransition?.Invoke(this);
        public override void AfterTransition() => _afterTransition?.Invoke(this);
    }
}