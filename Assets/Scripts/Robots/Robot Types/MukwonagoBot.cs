using UnityEngine;

public class MukwonagoBot : BaseRobot
{
    public override void CoralIntake()
    {
        Debug.Log("MukwonagoBot is intaking coral.");
    }

    public override void AlgaeIntake()
    {
        Debug.Log("MukwonagoBot is intaking algae.");
    }

    public override void CoralScore()
    {
        Debug.Log("MukwonagoBot is scoring coral.");
    }

    public override void NetScore() 
    {
        Debug.Log("MukwonagoBot is scoring in the net.");
    }

    public override void ProcessorScore()
    {
        Debug.Log("MukwonagoBot is scoring in the processor.");
    }

    public override void SuperScore()
    {
        Debug.Log("MukwonagoBot is performing a super score.");
    }

    public override void Climb()
    {
        Debug.Log("MukwonagoBot is climbing.");
    }

    public override void ClimberDown()
    {
        Debug.Log("MukwonagoBot is moving the climber down.");
    }

    public override void ClimberUp()
    {
        Debug.Log("MukwonagoBot is moving the climber up.");
    }

    public override void CoralEject()
    {
        Debug.Log("MukwonagoBot is ejecting coral.");
    }

    public override void AlgaeEject()
    {
        Debug.Log("MukwonagoBot is ejecting algae.");
    }
}
