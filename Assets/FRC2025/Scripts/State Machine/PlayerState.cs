namespace FRC2025
{
    public abstract class PlayerState
    {
        public BaseRobot BaseRobot;
        protected StateMachine _stateMachine;

        public PlayerState(BaseRobot robot, StateMachine stateMachine)
        {
            BaseRobot = robot;
            _stateMachine = stateMachine;

            BaseRobot = RobotHelper.GetBaseRobotScript(robot.gameObject);
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}