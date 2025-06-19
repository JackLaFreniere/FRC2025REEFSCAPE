using UnityEngine;
using UnityEngine.InputSystem;

public class KitBotInputInstructions : MonoBehaviour, IRobotInputHandler
{
    private KitBotInput playerControls;
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
        playerControls = new KitBotInput();
    }

    public void InputOnEnable()
    {
        playerControls.Player.Enable();

        drive = playerControls.Player.Drive;
        rotate = playerControls.Player.Rotate;

        playerControls.Player.CoralScore.performed += ctx => robot.CoralScore();
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