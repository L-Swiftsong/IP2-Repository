namespace HFSM.Exceptions
{
    public static class Common
    {
        public static StateMachineException NotInitialised(string context = null, string problem = null, string solution = null)
        {
            return new StateMachineException(ExceptionFormatter.Format(
                context,
                problem ?? "The active state is null because the State Machine has not been set up yet",
                solution ?? "Call FSM.SetStartState(...) and FSM.Init() or FSM.OnEnter() to initialise the State Machine."
                ));
        }

        public static StateMachineException StateNotFound(string stateName, string context = null, string problem = null, string solution = null)
        {
            return new StateMachineException(ExceptionFormatter.Format(
                context,
                problem ?? $"The state \"{stateName}\" has not been defined yet/doesn't exist.",
                solution ?? ("\n"
                    + "1. Check that you are passing the correct state and transitions.\n"
                    + "2. Add this state before calling Init/OnEnter/OnLogic/RequestStateChange/...")
                ));
        }

        public static StateMachineException MissingStartState(string context = null, string problem = null, string solution = null)
        {
            return new StateMachineException(ExceptionFormatter.Format(
                context,
                problem ?? "No start state is selected. The State Machine needs at least one state to function properly.",
                solution ?? "Make sure that there is at least one state in the State Machine before running Init() or OnEnter() by calling FSM.AddState(...)."
                ));
        }

        public static StateMachineException QuickIndexerMisusedForGettingState(string stateName)
        {
            return new StateMachineException(ExceptionFormatter.Format(
                context: "Getting a nested State Machine with the indexer",
                problem: "The selected state is not a State Machine.",
                solution: $"This method is only there for quickly accessing a nested state machine. To get the selected state, use GetState(\"{stateName}\")"
                ));
        }
    }
}