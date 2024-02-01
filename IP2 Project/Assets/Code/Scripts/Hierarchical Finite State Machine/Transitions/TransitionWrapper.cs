using System;

namespace HFSM
{
    /// <summary> A class that allows you to run additional functions (Companion Code) before and after the wrapped state's code.</summary>
    public class TransitionWrapper
    {
        public class WrappedTransition : TransitionBase
        {
           private Action<TransitionBase>
                _beforeOnEnter, _afterOnEnter,
                _beforeShouldTransition, _afterShouldTransition,
                _beforeOnExit, _afterOnExit;

            private TransitionBase _transition;


            public WrappedTransition(
                TransitionBase transition,
                Action<TransitionBase> beforeOnEnter,
                Action<TransitionBase> afterOnEnter,
                Action<TransitionBase> beforeShouldTransition,
                Action<TransitionBase> afterShouldTransition,
                Action<TransitionBase> beforeOnExit,
                Action<TransitionBase> afterOnExit
                ) : base(transition.From, transition.To, forceInstantly: transition.ForceInstantly)
            {
                this._transition = transition;

                this._beforeOnEnter = beforeOnEnter;
                this._afterOnEnter = afterOnEnter;
                
                this._beforeShouldTransition = beforeShouldTransition;
                this._afterShouldTransition = afterShouldTransition;

                this._beforeOnExit = beforeOnExit;
                this._afterOnExit = afterOnExit;
            }


            public override void Init() => _transition.FSM = this.FSM;

            public override void OnEnter()
            {
                _beforeOnEnter?.Invoke(this);
                _transition.OnEnter();
                _afterOnEnter?.Invoke(this);
            }
            public override void OnExit()
            {
                _beforeOnExit?.Invoke(this);
                _transition.OnExit();
                _afterOnExit?.Invoke(this);
            }

            public override bool ShouldTransition()
            {
                _beforeShouldTransition?.Invoke(this);
                bool shouldTransition = _transition.ShouldTransition();
                _afterShouldTransition?.Invoke(this);

                return shouldTransition;
            }

            public override void BeforeTransition() => _transition.BeforeTransition();
            public override void AfterTransition() => _transition.AfterTransition();
        }

        private Action<TransitionBase>
            _beforeOnEnter, _afterOnEnter,
            _beforeShouldTransition, _afterShouldTransition,
            _beforeOnExit, _afterOnExit;


        public TransitionWrapper(
            Action<TransitionBase> beforeOnEnter = null,
            Action<TransitionBase> afterOnEnter = null,
            Action<TransitionBase> beforeShouldTransition = null,
            Action<TransitionBase> afterShouldTransition = null,
            Action<TransitionBase> beforeOnExit = null,
            Action<TransitionBase> afterOnExit = null)
        {
            this._beforeOnEnter = beforeOnEnter;
            this._afterOnEnter = afterOnEnter;

            this._beforeShouldTransition = beforeShouldTransition;
            this._afterShouldTransition = afterShouldTransition;

            this._beforeOnExit = beforeOnExit;
            this._afterOnExit = afterOnExit;
        }


        public WrappedTransition Wrap(TransitionBase transition) => new WrappedTransition(transition, _beforeOnEnter, _afterOnEnter, _beforeShouldTransition, _afterShouldTransition, _beforeOnExit, _afterOnExit);
    }
}