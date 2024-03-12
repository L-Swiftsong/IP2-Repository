using System;

namespace HFSM
{
    /// <summary> A class that allows you to run additional functions (Companion Code) before and after the wrapped state's code.
    ///     It does not interfere with the wrapped state's timing/needsExitTime/etc behaviour.</summary>
    public class StateWrapper<TEvent>
    {
        public class WrappedState : IState, IActionable<TEvent>
        {
            // Companion Code Functions.
            private Action<IState>
                _beforeOnEnter, _afterOnEnter,
                _beforeOnLogic, _afterOnLogic,
                _beforeOnFixedLogic, _afterOnFixedLogic,
                _beforeOnExit, _afterOnExit;

            // The wrapped state.
            private IState _state;

            public string Name { get; }
            public bool NeedsExitTime { get; }
            public bool IsGhostState { get; }

            public IStateMachine FSM { get; set; }



            public WrappedState(
                IState state,
                
                Action<IState> beforeOnEnter = null,
                Action<IState> afterOnEnter = null,
                
                Action<IState> beforeOnLogic = null,
                Action<IState> afterOnLogic = null,

                Action<IState> beforeOnFixedLogic = null,
                Action<IState> afterOnFixedLogic = null,

                Action<IState> beforeOnExit = null,
                Action<IState> afterOnExit = null)
            {
                this._state = state;

                this.Name = state.Name;
                this.NeedsExitTime = state.NeedsExitTime;
                this.IsGhostState = state.IsGhostState;
                this.FSM = state.FSM;


                this._beforeOnEnter = beforeOnEnter;
                this._afterOnEnter = afterOnEnter;

                this._beforeOnLogic = beforeOnLogic;
                this._afterOnLogic = afterOnLogic;

                this._beforeOnFixedLogic = beforeOnFixedLogic;
                this._afterOnFixedLogic = afterOnFixedLogic;

                this._beforeOnExit = beforeOnExit;
                this._afterOnExit = afterOnExit;
            }


            public void Init()
            {
                // Override the wrapped state's default values.
                //_state.Name = Name;
                _state.FSM = FSM;

                // Initialise the wrapped state.
                _state.Init();
            }

            public void OnEnter()
            {
                _beforeOnEnter?.Invoke(this);
                _state.OnEnter();
                _afterOnEnter?.Invoke(this);
            }
            public void OnLogic()
            {
                _beforeOnLogic?.Invoke(this);
                _state.OnLogic();
                _afterOnLogic?.Invoke(this);
            }
            public void OnFixedLogic()
            {
                _beforeOnFixedLogic?.Invoke(this);
                _state.OnFixedLogic();
                _afterOnFixedLogic?.Invoke(this);
            }
            public void OnExit()
            {
                _beforeOnExit?.Invoke(this);
                _state.OnExit();
                _afterOnExit?.Invoke(this);
            }

            public void OnExitRequest() => _state.OnExitRequest();
            


            public void OnAction(TEvent trigger) => (_state as IActionable<TEvent>)?.OnAction(trigger);
            public void OnAction<TData>(TEvent trigger, TData data) => (_state as IActionable<TEvent>)?.OnAction<TData>(trigger, data);
        }

        // Companion Code Functions.
        private Action<IState>
            _beforeOnEnter, _afterOnEnter,
            _beforeOnLogic, _afterOnLogic,
            _beforeOnExit, _afterOnExit;


        /// <summary> Initialises a new instance of the StateWrapper class.</summary>
        public StateWrapper(
            Action<IState> beforeOnEnter = null,
            Action<IState> afterOnEnter = null,

            Action<IState> beforeOnLogic = null,
            Action<IState> afterOnLogic = null,

            Action<IState> beforeOnExit = null,
            Action<IState> afterOnExit = null)
        {
            this._beforeOnEnter = beforeOnEnter;
            this._afterOnEnter = afterOnEnter;

            this._beforeOnLogic = beforeOnLogic;
            this._afterOnLogic = afterOnLogic;

            this._beforeOnExit = beforeOnExit;
            this._afterOnExit = afterOnExit;
        }


        public WrappedState Wrap(IState state) => new WrappedState(state, _beforeOnEnter, _afterOnEnter, _beforeOnLogic, _afterOnLogic, _beforeOnExit, _afterOnExit);
    }


    #region Overloaded Classes
    // Overloaded Classes allow for an easier useage of the class for common cases.

    /// <inheritdoc/>
    public class StateWrapper : StateWrapper<string>
    {
        public StateWrapper(
            Action<IState> beforeOnEnter = null,
            Action<IState> afterOnEnter = null,

            Action<IState> beforeOnLogic = null,
            Action<IState> afterOnLogic = null,

            Action<IState> beforeOnExit = null,
            Action<IState> afterOnExit = null
            ) : base (beforeOnEnter, afterOnEnter, beforeOnLogic, afterOnLogic, beforeOnExit, afterOnExit)
        {

        }
    }
    #endregion
}