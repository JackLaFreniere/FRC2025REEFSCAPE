using UnityEngine;

public class MukwonagoBotManager : BaseRobot
{
#pragma warning disable IDE1006
    public Elevator elevator { get; private set; }
    public Shoulder shoulder { get; private set; }
    public Elbow elbow { get; private set; }
    public Wrist wrist { get; private set; }
    public Climber climber { get; private set; }
#pragma warning restore IDE1006

    private void Awake()
    {
        stateMachine = new StateMachine();
        stowState = new Stow(this, stateMachine);
        coralIntakeState = new MukwonagoBotCoralIntakeState(this, stateMachine);
        algaeIntakeState = new MukwonagoBotAlgaeIntakeState(this, stateMachine);
        coralScoreState = new MukwonagoBotCoralScoreState(this, stateMachine);
        netScoreState = new MukwonagoBotNetScoreState(this, stateMachine);
        processorScoreState = new MukwonagoBotProcessorScoreState(this, stateMachine);
        climberDownState = new MukwonagoBotClimberDownState(this, stateMachine);
        climberUpState = new MukwonagoBotClimberUpState(this, stateMachine);
        coralEjectState = new MukwonagoBotCoralEjectState(this, stateMachine);
        algaeEjectState = new MukwonagoBotAlgaeEjectState(this, stateMachine);
        confirmCoralScore = new MukwonagoBotConfirmCoralScoreState(this, stateMachine);
    }

    protected override void Start()
    {
        elevator = transform.Find("Elevator").GetComponent<Elevator>();
        shoulder = transform.Find("Elevator").Find("Shoulder").GetComponent<Shoulder>();
        elbow = transform.Find("Elevator").Find("Shoulder").Find("Elbow").GetComponent<Elbow>();
        wrist = transform.Find("Elevator").Find("Shoulder").Find("Elbow").Find("Wrist").GetComponent<Wrist>();
        climber = transform.Find("Climber").GetComponent<Climber>();

        base.Start();
        stateMachine.Initialize(stowState);
    }

    private void Update()
    {
        stateMachine.CurrentState.Update();
    }

    public override void CoralIntake()
    {
        if (hasAlgae || hasCoral) return;

        stateMachine.ChangeState(coralIntakeState);
    }

    public override void AlgaeIntake()
    {
        if (hasAlgae || hasCoral) return;

        stateMachine.ChangeState(algaeIntakeState);
    }

    public override void CoralScore()
    {
        if (hasAlgae) return;

        stateMachine.ChangeState(coralScoreState);
        Debug.Log("CoralScore");
    }

    public override void NetScore()
    {
        if (hasCoral) return;

        stateMachine.ChangeState(netScoreState);
    }

    public override void ProcessorScore()
    {
        if (hasCoral) return;

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

    public override void ConfirmCoralScore()
    {
        if (stateMachine.CurrentState is not MukwonagoBotCoralScoreState) return;

        stateMachine.ChangeState(confirmCoralScore);
        Debug.Log("ConfirmCoralScore");
    }

    public override void Stow()
    {
        stateMachine.ChangeState(stowState);
        Debug.Log("Stow");
    }
}