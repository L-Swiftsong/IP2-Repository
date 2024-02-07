using System;

namespace HFSM
{
    /*
     "Shortcut" Methods.
        - These are meant to reduce the biolerplate code required by the user for simple states and transitions.
        - They do this by setting a new State/Transition instance in the background and then setting the desired fields.
        - They can also optimise certain cases for you by chosing teh best type, such as a StateBase for an empty state instead of a State instance.
     */
    public static class StateMachineShortcuts
    {
        /// <summary> Creates the most efficient transition type possible for the given parameters.
        ///     It creates a Transition instance when a condition or transition callbacks are specified,
        ///     otherwise it returns a TransitionBase.</summary>
        private static TransitionBase CreateOptimisedTransition(
            IState from,
            IState to,
            Func<Transition, bool> condition = null,
            Action<Transition> onTransition = null,
            Action<Transition> afterTransition = null,
            bool forceInstantly = false)
        {
            // Optimise for empty transitions.
            if (condition == null && onTransition == null && afterTransition == null)
            {
                return new TransitionBase(from, to, forceInstantly);
            }

            return new Transition(
                from,
                to,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                );
        }


        #region Standard Transitions
        /// <summary> Shortcut method for adding a regular transition.
        ///     It creates a new Transition() under the hood (See Transition for more information).</summary>
        /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
        ///     otherwise it creates a Transition object.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
		/// 	Action{Transition},
        /// 	Action{Transition},
        /// 	bool
        /// )"/>
        public static void AddTransition<TEvent>(
            this StateMachine<TEvent> fsm,
            IState from,
            IState to,
            Func<Transition, bool> condition = null,
            Action<Transition> onTransition = null,
            Action<Transition> afterTransition = null,
            bool forceInstantly = false)
        {
            fsm.AddTransition(CreateOptimisedTransition(
                from,
                to,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                ));
        }

        /// <summary> Shortcut method for adding a regular transition that can happen from any state.
        ///     It creates a new Transition() under the hood (See Transition for more information).</summary>
        /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
        ///     otherwise it creates a Transition object.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
        ///     Action{Transition},
        ///     Action{Transition},
        ///     bool
        /// )"/>
        public static void AddAnyTransition<TEvent>(
            this StateMachine<TEvent> fsm,
            IState to,
            Func<Transition, bool> condition = null,
            Action<Transition> onTransition = null,
            Action<Transition> afterTransition = null,
            bool forceInstantly = false)
        {
            fsm.AddAnyTransition(CreateOptimisedTransition(
                default,
                to,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                ));
        }

        /// <summary> Shortcut method for adding a new trigger transition between two states
        ///     that is only checked if the specified trigger has been activated.</summary>
        /// <inheritdoc cref="TriggerTransition(
        ///     IState,
        ///     IState,
        ///     Action,
        ///     Func{TriggerTransition, bool},
		/// 	Action{TriggerTransition},
        /// 	Action{TriggerTransition},
        /// 	bool
        /// )"/>
        public static void AddTriggerTransition(
            this StateMachine<Action> fsm,
            IState from,
            IState to,
            ref Action trigger,
            Func<TriggerTransition, bool> condition = null,
            Action<TriggerTransition> onTransition = null,
            Action<TriggerTransition> afterTransition = null,
            bool forceInstantly = false)
        {
            fsm.AddTransition(new TriggerTransition(
                from,
                to,
                ref trigger,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                ));
        }

  //      /// <summary> Shortcut method for adding a new trigger transition that can happen from any possible state,
  //      ///     but is only checked when the specified trigger is activated.
  //      ///     It creates a new Transition() under the hood (See Transition for more information).</summary>
  //      /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
  //      ///     otherwise it creates a Transition object.</remarks>
  //      /// <inheritdoc cref="Transition(
  //      ///     IState,
  //      ///     IState,
  //      ///     Func{Transition, bool},
		///// 	Action{Transition},
  //      /// 	Action{Transition},
  //      /// 	bool
  //      /// )"/>
  //      public static void AddTriggerTransitionFromAny<TEvent>(
  //          this StateMachine<TEvent> fsm,
  //          TEvent trigger,
  //          IState to,
  //          Func<Transition, bool> condition = null,
  //          Action<Transition> onTransition = null,
  //          Action<Transition> afterTransition = null,
  //          bool forceInstantly = false)
  //      {
  //          fsm.AddTriggerTransitionFromAny(trigger, CreateOptimisedTransition(
  //              default,
  //              to,
  //              condition,
  //              onTransition: onTransition,
  //              afterTransition: afterTransition,
  //              forceInstantly: forceInstantly
  //              ));
  //      }


        /// <summary> Shortcut method for adding two transitions:
        ///     If the condition function is true, the FSM transition from the "From" state to the "To" state.
        ///     Otherwise, it performs a transition in the opposite direction ("To" to "From").</summary>
        /// <remarks> For the reverse transition the afterTransition callback is called before the transition and the onTransition callback afterwards.
        ///     If this is not desired then replicate the behaviour of the two way transitions by creating two separate transitions.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
		/// 	Action{Transition},
        /// 	Action{Transition},
        /// 	bool
        /// )"/>
        public static void AddTwoWayTransition<TEvent>(
            this StateMachine<TEvent> fsm,
            IState from,
            IState to,
            Func<Transition, bool> condition = null,
            Action<Transition> onTransition = null,
            Action<Transition> afterTransition = null,
            bool forceInstantly = false)
        {
            fsm.AddTwoWayTransition(new Transition(
                from,
                to,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                ));
        }

  //      /// <summary> Shortcut method for adding two transitions that are only checked when the specified trigger is activated:
  //      ///     If the condition function is true, the FSM transition from the "From" state to the "To" state.
  //      ///     Otherwise, it performs a transition in the opposite direction ("To" to "From").</summary>
  //      /// <remarks> For the reverse transition the afterTransition callback is called before the transition and the onTransition callback afterwards.
  //      ///     If this is not desired then replicate the behaviour of the two way transitions by creating two separate transitions.</remarks>
  //      /// <inheritdoc cref="Transition(
  //      ///     IState,
  //      ///     IState,
  //      ///     Func{Transition, bool},
		///// 	Action{Transition},
  //      /// 	Action{Transition},
  //      /// 	bool
  //      /// )"/>
  //      public static void AddTwoWayTriggerTransition<TEvent>(
  //          this StateMachine<TEvent> fsm,
  //          TEvent trigger,
  //          IState from,
  //          IState to,
  //          Func<Transition, bool> condition = null,
  //          Action<Transition> onTransition = null,
  //          Action<Transition> afterTransition = null,
  //          bool forceInstantly = false)
  //      {
  //          fsm.AddTwoWayTriggerTransition(trigger, new Transition(
  //              from,
  //              to,
  //              condition,
  //              onTransition: onTransition,
  //              afterTransition: afterTransition,
  //              forceInstantly: forceInstantly
  //              ));
  //      }
        #endregion


        #region Exit Transitions
        /// <summary> A shortcut method for adding a new exit transition from a state.
        ///     It represents an exit point that allows the FSM to exit and the parent FSM to continue to the next state.
        ///     It is only checked if the parent FSM has a pending transition.</summary>
        /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
        ///     otherwise it creates a Transition object.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
        /// 	Action{Transition},
        /// 	Action{Transition},
        /// 	bool
        /// )"/>
        public static void AddExitTransition<TEvent>(
            this StateMachine<TEvent> fsm,
            IState from,
            Func<Transition, bool> condition = null,
            Action<Transition> onTransition = null,
            Action<Transition> afterTransition = null,
            bool forceInstantly = false)
        {
            fsm.AddExitTransition(CreateOptimisedTransition(
                from,
                default,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                ));
        }

        /// <summary> A shortcut method for adding a new exit transition that can happen from any state.
        ///     It represents an exit point that allows the FSM to exit and the parent FSM to continue to the next state.
        ///     It is only checked if the parent FSM has a pending transition.</summary>
        /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
        ///     otherwise it creates a Transition object.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
		/// 	Action{Transition},
        /// 	Action{Transition},
        /// 	bool
        /// )"/>
        public static void AddExitAnyTransition<TEvent>(
            this StateMachine<TEvent> fsm,
            Func<Transition, bool> condition = null,
            Action<Transition> onTransition = null,
            Action<Transition> afterTransition = null,
            bool forceInstantly = false)
        {
            fsm.AddExitAnyTransition(CreateOptimisedTransition(
                default,
                default,
                condition,
                onTransition: onTransition,
                afterTransition: afterTransition,
                forceInstantly: forceInstantly
                ));
        }

        /// <summary> A shortcut method for adding a new exit transition from a state that is only checked when the specified trigger is activated.
        ///     It represents an exit point that allows the FSM to exit and the parent FSM to continue to the next state.
        ///     It is only checked if the parent FSM has a pending transition.</summary>
        /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
        ///     otherwise it creates a Transition object.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
		/// 	Action{Transition},
        /// 	Action{Transition},
        /// 	bool
        /// )"/>
        //public static void AddExitTriggerTransition<TEvent>(
        //    this StateMachine<TEvent> fsm,
        //    TEvent trigger,
        //    IState from,
        //    Func<Transition, bool> condition = null,
        //    Action<Transition> onTransition = null,
        //    Action<Transition> afterTransition = null,
        //    bool forceInstantly = false)
        //{
        //    fsm.AddExitTriggerTransition(trigger, CreateOptimisedTransition(
        //        from,
        //        default,
        //        condition,
        //        onTransition: onTransition,
        //        afterTransition: afterTransition,
        //        forceInstantly: forceInstantly
        //        ));
        //}

        /// <summary> A shortcut method for adding a new exit transition that can happen from any possible state and is only checked when the specified trigger is activated.
        ///     It represents an exit point that allows the FSM to exit and the parent FSM to continue to the next state.
        ///     It is only checked if the parent FSM has a pending transition.</summary>
        /// <remarks> When no condition or callbacks are required, it creates a TransitionBase for optimal performance,
        ///     otherwise it creates a Transition object.</remarks>
        /// <inheritdoc cref="Transition(
        ///     IState,
        ///     IState,
        ///     Func{Transition, bool},
		/// 	Action{Transition},
        /// 	Action{Transition},
        /// 	bool
        /// )"/>
        //public static void AddExitTriggerTransitionFromAny<TEvent>(
        //    this StateMachine<TEvent> fsm,
        //    TEvent trigger,
        //    Func<Transition, bool> condition = null,
        //    Action<Transition> onTransition = null,
        //    Action<Transition> afterTransition = null,
        //    bool forceInstantly = false)
        //{
        //    fsm.AddExitTriggerTransitionFromAny(trigger, CreateOptimisedTransition(
        //        default,
        //        default,
        //        condition,
        //        onTransition: onTransition,
        //        afterTransition: afterTransition,
        //        forceInstantly: forceInstantly
        //        ));
        //}
#endregion
    }
}