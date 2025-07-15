using UnityEngine;

public class KitBotManager : BaseRobot
{
    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new KitBotIdleState(this, stateMachine);
        coralScoreState = new KitBotCoralScoreState(this, stateMachine);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.CurrentState.Update();
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
