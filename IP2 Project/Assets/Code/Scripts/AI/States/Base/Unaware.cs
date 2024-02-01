using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Unaware : SubStateMachine
    {
        public override string Name { get => "Unaware"; }
    }
}