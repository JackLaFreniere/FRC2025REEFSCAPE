using UnityEngine;

public class KitBotManager : BaseRobot
{
    private void Awake()
    {
        stateMachine = new StateMachine();
        stowState = new KitBotIdleState(this, stateMachine);
        coralScoreState = new KitBotCoralScoreState(this, stateMachine);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(stowState);
    }

    private void Update()
    {
        stateMachine.CurrentState.Update();
    }

    public override void CoralScore()
    {
        stateMachine.ChangeState(coralScoreState);
    }

    public override void Stow()
    {
        stateMachine.ChangeState(stowState);
    }
}
