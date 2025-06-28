using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class MukwonagoBotManager : BaseRobot
{
    public Elevator elevator { get; private set; }
    public Shoulder shoulder { get; private set; }
    public Elbow elbow { get; private set; }
    public Wrist wrist { get; private set; }
    public Climber climber { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new MukwonagoBotIdleState(this, stateMachine);
        coralIntakeState = new MukwonagoBotCoralIntakeState(this, stateMachine);
        algaeIntakeState = new MukwonagoBotAlgaeIntakeState(this, stateMachine);
        coralScoreState = new MukwonagoBotCoralScoreState(this, stateMachine);
        netScoreState = new MukwonagoBotNetScoreState(this, stateMachine);
        processorScoreState = new MukwonagoBotProcessorScoreState(this, stateMachine);
        climberDownState = new MukwonagoBotClimberDownState(this, stateMachine);
        climberUpState = new MukwonagoBotClimberUpState(this, stateMachine);
        coralEjectState = new MukwonagoBotCoralEjectState(this, stateMachine);
        algaeEjectState = new MukwonagoBotAlgaeEjectState(this, stateMachine);
    }

    protected override void Start()
    {
        elevator = transform.Find("Elevator").GetComponent<Elevator>();
        shoulder = transform.Find("Elevator").Find("Shoulder").GetComponent<Shoulder>();
        elbow = transform.Find("Elevator").Find("Shoulder").Find("Elbow").GetComponent<Elbow>();
        wrist = transform.Find("Elevator").Find("Shoulder").Find("Elbow").Find("Wrist").GetComponent<Wrist>();
        climber = transform.Find("Climber").GetComponent<Climber>();

        base.Start();
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.CurrentState.Update();
    }

    public override void CoralIntake()
    {
        stateMachine.ChangeState(coralIntakeState);
    }

    public override void AlgaeIntake()
    {
        stateMachine.ChangeState(algaeIntakeState);
    }

    public override void CoralScore()
    {
        stateMachine.ChangeState(coralScoreState);
    }

    public override void NetScore()
    {
        stateMachine.ChangeState(netScoreState);
    }

    public override void ProcessorScore()
    {
        stateMachine.ChangeState(processorScoreState);
    }

    public override void ClimberDown()
    {
        stateMachine.ChangeState(climberDownState);
    }

    public override void ClimberUp()
    {
        stateMachine.ChangeState(climberUpState);
    }

    public override void CoralEject()
    {
        stateMachine.ChangeState(coralEjectState);
    }

    public override void AlgaeEject()
    {
        stateMachine.ChangeState(algaeEjectState);
    }

    public override void Idle()
    {
        stateMachine.ChangeState(idleState);
    }
}