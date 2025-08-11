public class Stow : PlayerState
{
private readonly MukwonagoBotManager bot;

public Stow(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
{
    bot = (MukwonagoBotManager) player;
}


public override void Enter()
{
    if (BaseRobot.hasAlgae)
    {
        bot.shoulder.SetTargetRotation(60f);
        bot.elbow.SetTargetRotation(155f);
        bot.wrist.SetTargetRotation(0f);
        bot.elevator.SetTargetPosition(0f);
    }
    else
    {
        bot.shoulder.SetTargetRotation(90f);
        bot.elbow.SetTargetRotation(65f);
        bot.wrist.SetTargetRotation(0f);
        bot.elevator.SetTargetPosition(0f);
    }
}
}

public class MukwonagoBotCoralIntakeState : PlayerState
{
private readonly MukwonagoBotManager bot;

public MukwonagoBotCoralIntakeState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
{
    bot = (MukwonagoBotManager) player;
}

public override void Enter()
{
    bot.shoulder.SetTargetRotation(121.5f);
    bot.elbow.SetTargetRotation(142.5f);
    bot.wrist.SetTargetRotation(-90f);
    bot.elevator.SetTargetPosition(0f);
}
}

public class MukwonagoBotAlgaeIntakeState : PlayerState
{
private readonly MukwonagoBotManager bot;

public MukwonagoBotAlgaeIntakeState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
{
    bot = (MukwonagoBotManager)player;
}

public override void Enter()
{
    AlgaeReefLevel algaeReefLevel = MukwonagoBotPresets.algaeReefLevel;

    switch (algaeReefLevel)
    {
        case AlgaeReefLevel.low:
            bot.shoulder.SetTargetRotation(15f);
            bot.elbow.SetTargetRotation(100f);
            bot.wrist.SetTargetRotation(0f);
            bot.elevator.SetTargetPosition(6f); 
            break;
        case AlgaeReefLevel.high:
            bot.shoulder.SetTargetRotation(-35f);
            bot.elbow.SetTargetRotation(57f);
            bot.wrist.SetTargetRotation(0f);
            bot.elevator.SetTargetPosition(7f);
            break;
    }
}
}

public class MukwonagoBotCoralScoreState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotCoralScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        CoralReefLevel coralReefLevel = MukwonagoBotPresets.coralReefLevel;

        switch (coralReefLevel)
        {
            case CoralReefLevel.L1:
                bot.shoulder.SetTargetRotation(55f);
                bot.elbow.SetTargetRotation(-40f);
                bot.wrist.SetTargetRotation(90f);
                bot.elevator.SetTargetPosition(0f); 
                break;
            case CoralReefLevel.L2:
                bot.shoulder.SetTargetRotation(35f);
                bot.elbow.SetTargetRotation(20f);
                bot.wrist.SetTargetRotation(0f);
                bot.elevator.SetTargetPosition(0f); 
                break;
            case CoralReefLevel.L3:
                bot.shoulder.SetTargetRotation(32.5f);
                bot.elbow.SetTargetRotation(20f);
                bot.wrist.SetTargetRotation(0f);
                bot.elevator.SetTargetPosition(15f); 
                break;
            case CoralReefLevel.L4:
                bot.shoulder.SetTargetRotation(-60f);
                bot.elbow.SetTargetRotation(-100f);
                bot.wrist.SetTargetRotation(0f);
                bot.elevator.SetTargetPosition(18f); 
                break;
        }
    }
}

public class MukwonagoBotNetScoreState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotNetScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        bot.shoulder.SetTargetRotation(-85f);
        bot.elbow.SetTargetRotation(65f);
        bot.wrist.SetTargetRotation(0f);
        bot.elevator.SetTargetPosition(26f);
    }
}

public class MukwonagoBotProcessorScoreState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotProcessorScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        bot.shoulder.SetTargetRotation(50f);
        bot.elbow.SetTargetRotation(120f);
        bot.wrist.SetTargetRotation(0f);
        bot.elevator.SetTargetPosition(0f);
    }
}

public class MukwonagoBotClimberDownState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotClimberDownState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        bot.climber.SetTargetRotation(158f);
    }
}

public class MukwonagoBotClimberUpState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotClimberUpState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        bot.climber.SetTargetRotation(0f);// 80f);
    }
}

public class MukwonagoBotCoralEjectState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotCoralEjectState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        bot.wrist.GetComponentInChildren<CoralIntakeZone>().EjectCoral();
    }
}

public class MukwonagoBotAlgaeEjectState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotAlgaeEjectState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        bot.elbow.GetComponentInChildren<AlgaeIntakeZone>().EjectAlgae();
    }
}

public class MukwonagoBotConfirmCoralScoreState : PlayerState
{
    private readonly MukwonagoBotManager bot;

    public MukwonagoBotConfirmCoralScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine)
    {
        bot = (MukwonagoBotManager)player;
    }

    public override void Enter()
    {
        CoralReefLevel coralReefLevel = MukwonagoBotPresets.coralReefLevel;

        switch (coralReefLevel)
        {
            case CoralReefLevel.L2:
                bot.shoulder.SetTargetRotation(70f);
                bot.elbow.SetTargetRotation(-20f);
                break;
            case CoralReefLevel.L3:
                bot.shoulder.SetTargetRotation(47f);
                bot.elbow.SetTargetRotation(-50f); 
                break;
            case CoralReefLevel.L4:
                bot.elbow.SetTargetRotation(-150f);
                break;
        }
    }

    public override void Update()
    {
        if (bot.shoulder.IsAtTargetRotation() && bot.elbow.IsAtTargetRotation() && bot.elevator.IsAtTargetPosition() && bot.wrist.IsAtTargetRotation() && BaseRobot.hasCoral)
        {
            bot.wrist.GetComponentInChildren<CoralIntakeZone>().EjectCoral();
        }
    }
}