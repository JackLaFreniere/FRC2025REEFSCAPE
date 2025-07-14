using UnityEngine;

public class MukwonagoBotInputInstructions : MonoBehaviour, IRobotInputHandler
{
    private MukwonagoBotInput playerControls;
    private BaseRobot robot;

    public void SetBaseRobot(BaseRobot robot)
    {
        this.robot = robot;
    }

    public void InputAwake()
    {
        playerControls = new MukwonagoBotInput();
    }

    public void InputOnEnable()
    {
        playerControls.Player.Enable();

        playerControls.Player.CoralIntake.performed += ctx => robot.CoralIntake();
        playerControls.Player.AlgaeIntake.performed += ctx => robot.AlgaeIntake();
        playerControls.Player.CoralScore.performed += ctx => robot.CoralScore();
        playerControls.Player.NetScore.performed += ctx => robot.NetScore();
        playerControls.Player.ProcessorScore.performed += ctx => robot.ProcessorScore();
        playerControls.Player.ClimberDown.performed += ctx => robot.ClimberDown();
        playerControls.Player.ClimberUp.performed += ctx => robot.ClimberUp();
        playerControls.Player.CoralEject.performed += ctx => robot.CoralEject();
        playerControls.Player.AlgaeEject.performed += ctx => robot.AlgaeEject();
        playerControls.Player.ConfirmScore.performed += ctx => robot.ConfirmCoralScore();

        playerControls.Player.CoralIntake.canceled += ctx => robot.Idle();
        playerControls.Player.AlgaeIntake.canceled += ctx => robot.Idle();
        playerControls.Player.CoralScore.canceled += ctx => robot.Idle();
        playerControls.Player.NetScore.canceled += ctx => robot.Idle();
        playerControls.Player.ProcessorScore.canceled += ctx => robot.Idle();
        playerControls.Player.ClimberDown.canceled += ctx => robot.Idle();
        playerControls.Player.ClimberUp.canceled += ctx => robot.Idle();
        playerControls.Player.CoralEject.canceled += ctx => robot.Idle();
        playerControls.Player.AlgaeEject.canceled += ctx => robot.Idle();
        playerControls.Player.ConfirmScore.canceled += ctx => robot.CoralScore();

        robot.SetDrive(playerControls.Player.Drive);
        robot.SetRotate(playerControls.Player.Rotate);
    }

    public void InputOnDisable()
    {
        playerControls.Player.Disable();
    }
}