using UnityEngine;
using UnityEngine.InputSystem;

public class MukwonagoBotInputInstructions : MonoBehaviour, IRobotInputHandler
{
    private MukwonagoBotInput playerControls;
    private BaseRobot robot;

    private InputAction drive;
    private InputAction rotate;

    private Vector2 moveDirection;
    private Vector2 rotateDirection;

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

        drive = playerControls.Player.Drive;
        rotate = playerControls.Player.Rotate;

        playerControls.Player.AlgaeIntake.performed += ctx => robot.AlgaeIntake();
        playerControls.Player.CoralIntake.performed += ctx => robot.CoralIntake();
        playerControls.Player.CoralScore.performed += ctx => robot.CoralScore();
        playerControls.Player.NetScore.performed += ctx => robot.NetScore();
        playerControls.Player.ProcessorScore.performed += ctx => robot.ProcessorScore();
        playerControls.Player.SuperScore.performed += ctx => robot.SuperScore();
        playerControls.Player.ClimberDown.performed += ctx => robot.ClimberDown();
        playerControls.Player.ClimberUp.performed += ctx => robot.ClimberUp();
        playerControls.Player.CoralEject.performed += ctx => robot.CoralEject();
        playerControls.Player.AlgaeEject.performed += ctx => robot.AlgaeEject();
    }

    public void InputOnDisable()
    {
        playerControls.Player.Disable();
    }

    public void InputFixedUpdate()
    {
        moveDirection = drive.ReadValue<Vector2>();
        rotateDirection = rotate.ReadValue<Vector2>();
        robot.Drive(moveDirection);
        robot.Rotate(rotateDirection);
    }
}