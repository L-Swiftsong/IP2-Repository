using System;

namespace HFSM
{
    /// <summary> A StateMachine that is also like a normal State in the sense that it allows you to run
    ///     custom code on enter, on logic, etc, besides its active state's code.
    /// It is especially handy for Hierarchical State Machines, as it allows you to factor out
    ///     common code from the substates into the HybridStateMachines, essentially removing
    ///     duplicate code.
    /// The HybridStateMachine can also be seen as a StateWrapper around a normal StateMachine.
    /// </summary>
    public abstract class HybridStateMachine<TEvent> : SubStateMachine<TEvent>
    {
        private Action<HybridStateMachine<TEvent>>
            _beforeOnEnter, _afterOnEnter,
            _beforeOnLogic, _afterOnLogic,
            _beforeOnExit, _afterOnExit;

        // Lazily initialised (Creation deferred until first use).
        private ActionStorage<TEvent> _actionStorage;


        public Timer timer;


        /// <summary> Initialises a new instance of the HybridStateMachine class.</summary>
        /// <param name="beforeOnEnter"> A function that is called before running the sub-state's OnEnter.</param>
        /// <param name="afterOnEnter"> A function that is called after running the sub-state's OnEnter.</param>
        /// <param name="beforeOnLogic"> A function that is called before running the sub-state's OnLogic</param>
        /// <param name="afterOnLogic"> A function that is called after running the sub-state's OnLogic.</param>
        /// <param name="beforeOnExit"> A function that is called before running the sub-state's OnExit</param>
        /// <param name="afterOnExit"> A function that is called after running the sub-state's OnExit.</param>
        /// <param name="needsExitTime"> (Only for Hierarchical States):
        ///     Determines whether the State Machine, as a state of a parent State Machine, is allowed to instantly
        ///     exit on a transition (False), or if it should wait until an explicit exit transition occurs (True).</param>
        /// <inheritdoc cref="StateBase{T}(bool, bool)"/>
        public HybridStateMachine(
            Action<HybridStateMachine<TEvent>> beforeOnEnter,
            Action<HybridStateMachine<TEvent>> afterOnEnter,

            Action<HybridStateMachine<TEvent>> beforeOnLogic,
            Action<HybridStateMachine<TEvent>> afterOnLogic,

            Action<HybridStateMachine<TEvent>> beforeOnExit,
            Action<HybridStateMachine<TEvent>> afterOnExit,
            
            bool needsExitTime = false, bool isGhostState = false) : base(needsExitTime, isGhostState)
        {
            this._beforeOnEnter = beforeOnEnter;
            this._afterOnEnter = afterOnEnter;

            this._beforeOnLogic = beforeOnLogic;
            this._afterOnLogic = afterOnLogic;

            this._beforeOnExit = beforeOnExit;
            this._afterOnExit = afterOnExit;
        }


        public override void OnEnter()
        {
            _beforeOnEnter?.Invoke(this);
            base.OnEnter();

            timer.Reset();
            _afterOnEnter?.Invoke(this);
        }

        public override void OnLogic()
        {
            _beforeOnLogic?.Invoke(this);
            base.OnLogic();
            _afterOnLogic?.Invoke(this);
        }

        public override void OnExit()
        {
            _beforeOnExit?.Invoke(this);
            base.OnExit();
            _afterOnExit?.Invoke(this);
        }


        public override void OnAction(TEvent trigger)
        {
            _actionStorage?.RunAction(trigger);
            base.OnAction(trigger);
        }
        public override void OnAction<TData>(TEvent trigger, TData data)
        {
            _actionStorage?.RunAction(trigger, data);
            base.OnAction(trigger, data);
        }


        /// <summary> Adds an action that can be called with OnAction().
        ///     Actions are like the built-in events OnEnter/OnLogic/etc, but are defined by the user.
        ///     The action is run before the sub-state's action.</summary>
        /// <param name="trigger"> The name of the action.</param>
        /// <param name="action"> The function that should be called when the action is run.</param>
        /// <returns> Itself.</returns>
        public HybridStateMachine<TEvent> AddAction(TEvent trigger, Action action)
        {
            // If we have no action storage, create one.
            _actionStorage = _actionStorage ?? new ActionStorage<TEvent>();
            
            // Add the action to the action storage.
            _actionStorage.AddAction(trigger, action);


            // Fluent interface.
            return this;
        }

        /// <summary> Adds an action that can be called with OnAction<T>().
        ///     This overload allows you to run a function that takes one data parameter.
        ///     The action is run before the sub-state's action.</summary>
        /// <param name="trigger"> The name of the action.</param>
        /// <param name="action"> The function that should be called when the action is run.</param>
        /// <typeparam name="TData"> Data type of the parameter of the function.</typeparam>
        /// <returns> Itself.</returns>
        public HybridStateMachine<TEvent> AddAction<TData>(TEvent trigger, Action<TData> action)
        {
            // If we have no action storage, create one.
            _actionStorage = _actionStorage ?? new ActionStorage<TEvent>();

            // Add the action to the action storage.
            _actionStorage.AddAction<TData>(trigger, action);


            // Fluent interface.
            return this;
        }
    }


    #region Overloaded Classes
    // Overloaded classes to allow for an easier useage of the HybridStateMachine for common cases.
    // (E.g. "new HybridStateMachine()" instead of "new HybridStateMachine<string, string, string>()").
    
    /// <inheritdoc/>
    public abstract class HybridStateMachine : HybridStateMachine<string>
    {
        /// <inheritdoc/>
        public HybridStateMachine(
            Action<HybridStateMachine<string>> beforeOnEnter = null,
            Action<HybridStateMachine<string>> afterOnEnter = null,

            Action<HybridStateMachine<string>> beforeOnLogic = null,
            Action<HybridStateMachine<string>> afterOnLogic = null,

            Action<HybridStateMachine<string>> beforeOnExit = null,
            Action<HybridStateMachine<string>> afterOnExit = null,

            bool needsExitTime = false, bool isGhostState = false) : base(
                beforeOnEnter, afterOnEnter,
                beforeOnLogic, afterOnLogic,
                beforeOnExit, afterOnExit,
                needsExitTime, isGhostState)
        {

        }
    }
    #endregion
}