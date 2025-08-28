namespace FRC2025
{
    public abstract class PlayerState
    {
        public BaseRobot player;
        protected StateMachine stateMachine;
        protected BaseRobot _baseRobot;

        public PlayerState(BaseRobot player, StateMachine stateMachine)
        {
            this.player = player;
            this.stateMachine = stateMachine;

            _baseRobot = RobotHelper.GetBaseRobotScript(player.gameObject);
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}