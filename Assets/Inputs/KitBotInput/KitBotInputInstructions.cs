using UnityEngine;
using UnityEngine.InputSystem;

public class KitBotInputInstructions : MonoBehaviour, IRobotInputHandler
{
    private KitBotInput playerControls;
    private BaseRobot robot;

    public void SetBaseRobot(BaseRobot robot)
    {
        this.robot = robot;
    }

    public void InputAwake()
    {
        playerControls = new KitBotInput();
    }

    public void InputOnEnable()
    {
        playerControls.Player.Enable();

        playerControls.Player.CoralScore.performed += ctx => robot.CoralScore();
        playerControls.Player.CoralScore.canceled += ctx => robot.Idle();

        robot.SetDrive(playerControls.Player.Drive);
        robot.SetRotate(playerControls.Player.Rotate);
    }

    public void InputOnDisable()
    {
        playerControls.Player.Disable();
    }
}