using System;
using System.Collections.Generic;
using HFSM.Exceptions;

namespace HFSM
{
    /// <summary> A class that can store and run actions. It makes implementing an action system easier in various state classes.</summary>
    public class ActionStorage<TEvent>
    {
        private Dictionary<TEvent, Delegate> _actionsByEvent = new Dictionary<TEvent, Delegate>();


        /// <summary> Returns the action belonging to the specified event.
        ///     If it does not exist, null is returned.
        ///     If the type of the existing action does not match the desired type, an exception is thrown.</summary>
        /// <param name="trigger"> The name of the action.</param>
        /// <typeparam name="TTarget"> Type of the function (delegate) belonging to the action.</typeparam>
        /// <returns> The action with the specified name.</returns>
        private TTarget TryGetAndCastAction<TTarget>(TEvent trigger) where TTarget : Delegate
        {
            Delegate action = null;
            _actionsByEvent.TryGetValue(trigger, out action);

            // If the action does not exist, return null.
            if (action is null)
                return null;

            TTarget target = action as TTarget;

            // If the type of the existing action doesn't match the desired type, throw an exception.
            if (target is null)
            {
                throw new InvalidOperationException(ExceptionFormatter.Format(
                    context: $"Trying to call the action '{trigger}'.",
                    problem: $"The expected argument ({typeof(TTarget)}) does not match the type of the added action ({action}).",
                    solution: "Check that the type of action that was added matches the type of action that was called. \n"
                        + "E.g. AddAction<int>(...) => OnAction<int>(...)\n"
                        + "E.g. AddAction(...) => OnAction(...)\n"
                        + "E.g. NOT: AddAction<int>(...) => OnAction<bool>(...)"
                    ));
            }

            // Return the found action.
            return target;
        }


        /// <summary> Adds an action that can be called with OnAction().
        /// Actions are like the built-in events OnEnter/OnLogic/etc, but are defined by the user.</summary>
        /// <param name="trigger"> The name of the action.</param>
        /// <param name="action"> The function that should be called when the action is run.</param>
        public void AddAction(TEvent trigger, Action action)
        {
            _actionsByEvent[trigger] = action;
        }


        /// <summary> Adds an action that can be called with RunAction<T>().
        ///     This overload allows you to run a function that takes one parameter</summary>
        /// <param name="trigger"> The name of thea ction.</param>
        /// <param name="action"> The function that should be called when the action is run.</param>
        /// <typeparam name="TData"> Data type of the parameter of the function.</typeparam>
        public void AddAction<TData>(TEvent trigger, Action<TData> action)
        {
            _actionsByEvent[trigger] = action;
        }



        /// <summary> Runs an action with the given name.
        ///     If the action is not defined/hasn't been added, nothing will happen.</summary>
        /// <param name="trigger"> The name of the action.</param>
        public void RunAction(TEvent trigger) => TryGetAndCastAction<Action>(trigger)?.Invoke();


        /// <summary> Runs an action with a given name and lets you pass in one parameter to the action function.
        ///     If the action is not defined/hasn't been added, nothing will happen.</summary>
        /// <param name="trigger"> The name of the action.</param>
        /// <param name="data"> Data to pass as the first parameter to the action.</param>
        /// <typeparam name="TData"> Type of the data parameter.</typeparam>
        public void RunAction<TData>(TEvent trigger, TData data) => TryGetAndCastAction<Action<TData>>(trigger)?.Invoke(data);
    }
}