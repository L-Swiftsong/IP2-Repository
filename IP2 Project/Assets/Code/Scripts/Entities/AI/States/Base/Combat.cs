using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Combat : SubStateMachine
    {
        public override string Name { get => "Combat"; }
    }
}