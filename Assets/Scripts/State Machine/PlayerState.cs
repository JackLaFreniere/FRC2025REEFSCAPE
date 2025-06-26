using UnityEngine;

public abstract class PlayerState
{
    public BaseRobot player;
    protected StateMachine stateMachine;

    public PlayerState(BaseRobot player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}