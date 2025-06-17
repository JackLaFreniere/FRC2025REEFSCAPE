using UnityEngine;

public class MukwonagoBot : BaseRobot, IRobotActions
{
    public void CoralIntake()
    {
        Debug.Log("MukwonagoBot is intaking coral.");
    }

    public void AlgaeIntake()
    {
        Debug.Log("MukwonagoBot is intaking algae.");
    }

    public void CoralScore()
    {
        Debug.Log("MukwonagoBot is scoring coral.");
    }

    public void NetScore() 
    {
        Debug.Log("MukwonagoBot is scoring in the net.");
    }

    public void ProcessorScore()
    {
        Debug.Log("MukwonagoBot is scoring in the processor.");
    }

    public void Climb()
    {
        Debug.Log("MukwonagoBot is climbing.");
    }
}
