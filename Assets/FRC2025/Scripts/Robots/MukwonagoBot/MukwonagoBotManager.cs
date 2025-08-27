public class MukwonagoBotManager : BaseRobot
{
#pragma warning disable IDE1006
    public TranslatingSubsystem elevator { get; private set; }
    public RotatingSubsystem shoulder { get; private set; }
    public RotatingSubsystem elbow { get; private set; }
    public RotatingSubsystem wrist { get; private set; }
    public RotatingSubsystem climber { get; private set; }
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
        elevator = transform.Find("Elevator").GetComponent<TranslatingSubsystem>();
        shoulder = transform.Find("Shoulder").GetComponent<RotatingSubsystem>();
        elbow = transform.Find("Elbow").GetComponent<RotatingSubsystem>();
        wrist = transform.Find("Wrist").GetComponent<RotatingSubsystem>();
        climber = transform.Find("Climber").GetComponent<RotatingSubsystem>();

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
    }

    public override void Stow()
    {
        stateMachine.ChangeState(stowState);
    }

    public override void TogglePreset()
    {
        MukwonagoBotPresets.CycleReefLevel();
    }     
}