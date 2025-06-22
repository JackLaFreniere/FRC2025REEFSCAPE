using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class CoralScoreState : PlayerState
{
    private GameObject scoringMechanism;
    private readonly float rotationSpeed = 360f;
    private Vector3 rotationAxis = Vector3.left;
    private GameObject[] scoringWheels;


    public CoralScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        scoringMechanism = player.gameObject.transform.Find("Coral_Scoring_Mechanism").gameObject;

        scoringWheels = GameObject.FindGameObjectsWithTag("CoralScoringWheel");

        foreach (GameObject wheel in scoringWheels)
        {
            wheel.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void UpdateLogic()
    {
        scoringMechanism.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    public override void Exit()
    {
        foreach (GameObject wheel in scoringWheels)
        {
            wheel.GetComponent<CapsuleCollider>().enabled = true;
        }
    }
}