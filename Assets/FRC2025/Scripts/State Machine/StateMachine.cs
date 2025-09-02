namespace FRC2025
{
    public class StateMachine
    {
        public PlayerState PreviousState { get; private set; }
        public PlayerState CurrentState { get; private set; }

        public void Initialize(PlayerState startState)
        {
            CurrentState = startState;
            CurrentState.Enter();
        }

        public void ChangeState(PlayerState newState)
        {
            PreviousState = CurrentState;
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}