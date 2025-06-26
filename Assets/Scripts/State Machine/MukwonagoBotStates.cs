using Unity.VisualScripting;
using UnityEngine;

public class MukwonagoBotIdleState : PlayerState
{
    public MukwonagoBotIdleState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    private Elevator elevator;
    private Shoulder shoulder;
    private Elbow elbow;
    private Wrist wrist;

    public override void Enter()
    {
        elevator = player.transform.Find("Elevator").GetComponent<Elevator>();
        shoulder = player.transform.Find("Elevator").Find("Shoulder").GetComponent<Shoulder>();
        elbow = player.transform.Find("Elevator").Find("Shoulder").Find("Elbow").GetComponent<Elbow>();
        wrist = player.transform.Find("Elevator").Find("Shoulder").Find("Elbow").Find("Wrist").GetComponent<Wrist>();
        elevator.SetTargetRotation(0f);
        shoulder.SetTargetRotation(0f);
        elbow.SetTargetRotation(0f);
        wrist.SetTargetRotation(0f);
    }
}

public class MukwonagoBotCoralIntakeState : PlayerState
{
    public MukwonagoBotCoralIntakeState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    private Elevator elevator;
    private Shoulder shoulder;
    private Elbow elbow;
    private Wrist wrist;

    public override void Enter()
    {
        elevator = player.transform.Find("Elevator").GetComponent<Elevator>();
        shoulder = player.transform.Find("Elevator").Find("Shoulder").GetComponent<Shoulder>();
        elbow = player.transform.Find("Elevator").Find("Shoulder").Find("Elbow").GetComponent<Elbow>();
        wrist = player.transform.Find("Elevator").Find("Shoulder").Find("Elbow").Find("Wrist").GetComponent<Wrist>();
        elevator.SetTargetRotation(0f);
        shoulder.SetTargetRotation(-30f);
        elbow.SetTargetRotation(45f);
        wrist.SetTargetRotation(90f);
    }
}

public class MukwonagoBotAlgaeIntakeState : PlayerState
{
    public MukwonagoBotAlgaeIntakeState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the algae intake state
        Debug.Log("MukwonagoBot is now in Algae Intake State.");
    }
}

public class MukwonagoBotCoralScoreState : PlayerState
{
    public MukwonagoBotCoralScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the coral score state
        Debug.Log("MukwonagoBot is now in Coral Score State.");
    }
}

public class MukwonagoBotNetScoreState : PlayerState
{
    public MukwonagoBotNetScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the net score state
        Debug.Log("MukwonagoBot is now in Net Score State.");
    }
}

public class MukwonagoBotProcessorScoreState : PlayerState
{
    public MukwonagoBotProcessorScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the processor score state
        Debug.Log("MukwonagoBot is now in Processor Score State.");
    }
}

public class MukwonagoBotClimberDownState : PlayerState
{
    public MukwonagoBotClimberDownState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the climber down state
        Debug.Log("MukwonagoBot is now in Climber Down State.");
    }
}

public class MukwonagoBotClimberUpState : PlayerState
{
    public MukwonagoBotClimberUpState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the climber up state
        Debug.Log("MukwonagoBot is now in Climber Up State.");
    }
}

public class MukwonagoBotCoralEjectState : PlayerState
{
    public MukwonagoBotCoralEjectState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the coral eject state
        Debug.Log("MukwonagoBot is now in Coral Eject State.");
    }
}

public class MukwonagoBotAlgaeEjectState : PlayerState
{
    public MukwonagoBotAlgaeEjectState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // Logic for entering the algae eject state
        Debug.Log("MukwonagoBot is now in Algae Eject State.");
    }
}