using System;
using System.Collections;
using UnityEngine;

namespace HFSM
{
    /// <summary> A state that can run a UnityCoroutine as its OnLogic method.</summary>
    public abstract class CoState : IState
    {
        public abstract string Name { get; }

        public bool NeedsExitTime { get; }
        public bool IsGhostState { get; }

        public IStateMachine FSM { get; set; }


        protected MonoBehaviour Mono;

        protected Coroutine ActiveCoroutine;
        protected bool ShouldLoopCoroutine;


        // The CoState class allows you to use either a function without any parameters
        //  or a function that takes the state as a parameter to create the coroutine.
        //  To allow for this and ease of use, the class has two nearly identical constructors.

        /// <summary> Initialises a new instance of the CoState class.</summary>
        /// <param name="mono"> The MonoBehaviour of the script that should run the coroutine.</param>
        /// <param name="loop"> If true, it will loop the coroutine, running it again once it has completed.</param>
        /// <param name="needsExitTime"> Determines if a state is allowed to instantly exit on a transition (False),
        ///     or if the State Machine should wait for until the State is ready for a state change (True).</param>
        /// <param name="isGhostState">If true, this state becomes a Ghost State, which the FSM doesn't want to stay in,
        ///     and will test all outgoing transitions instantly, as opposed to waiting for the next OnLogic call.</param>
        public CoState(MonoBehaviour mono, bool loop = true, bool needsExitTime = false, bool isGhostState = false)
        {
            this.Mono = mono;
            this.ShouldLoopCoroutine = loop;

            this.NeedsExitTime = needsExitTime;
            this.IsGhostState = isGhostState;
        }


        public virtual void Init() { }
        public virtual void OnEnter() => ActiveCoroutine = Mono.StartCoroutine(ShouldLoopCoroutine ? LoopCoroutine() : PrimaryCoroutine());
        
        public virtual void OnLogic()
        {
            // If we can exit this state, and the FSM is awaiting a transition, tell the FSM that we can transition.
            if (NeedsExitTime && FSM.HasPendingTransition && CanExit())
                FSM.StateCanExit();
        }
        public virtual void OnFixedLogic() { }

        public virtual void OnExit()
        {
            // Stop the coroutine if it is still active.
            if (ActiveCoroutine != null)
            {
                Mono.StopCoroutine(ActiveCoroutine);
                ActiveCoroutine = null;
            }
        }
        public virtual void OnExitRequest()
        {
            if (CanExit())
                FSM.StateCanExit();
        }
        protected virtual bool CanExit() => true;


        private IEnumerator LoopCoroutine()
        {
            IEnumerator routine = PrimaryCoroutine();

            while (true)
            {
                // This checks if the coroutine needs at least one frame to execute.
                // If not, LoopCoroutine will wait 1 frame to avoid an infinite loop which will crash Unity.
                if (routine.MoveNext())
                    yield return routine.Current;
                else
                    yield return null;

                // Iterate from the onLogic coroutine until it is depleted.
                while(routine.MoveNext())
                    yield return routine.Current;


                // Restart the coroutine.
                routine = PrimaryCoroutine();
            }
        }
        protected abstract IEnumerator PrimaryCoroutine();
    }
}