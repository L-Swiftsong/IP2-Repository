namespace HFSM
{
    public interface ITransitionListener
    {
        void BeforeTransition();
        void AfterTransition();
    }
}