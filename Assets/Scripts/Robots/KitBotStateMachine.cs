using UnityEngine;

public class KitBotStateMachine : BaseRobot
{
    private StateMachine stateMachine;
    private PlayerState idleState;
    private PlayerState coralScoreState;

    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new IdleState(this, stateMachine);
        coralScoreState = new CoralScoreState(this, stateMachine);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.UpdateLogic();
    }

    public override void CoralScore()
    {
        stateMachine.ChangeState(coralScoreState);
    }

    public override void Idle()
    {
        stateMachine.ChangeState(idleState);
    }
}
