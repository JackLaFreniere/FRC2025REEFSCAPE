using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActions playerControls;
    private Vector2 moveDirection;
    private InputAction drive;
    private InputAction coralScore;
    private IRobotActions robotActions;

    [SerializeField] private BaseRobot robot;

    private void Awake()
    {
        playerControls = new InputActions();
    }

    private void Start()
    {
        robotActions = robot.GetComponent<IRobotActions>();
    }

    private void OnEnable()
    {
        drive = playerControls.Player.Drive;
        drive.Enable();

        coralScore = playerControls.Player.CoralScore;
        coralScore.Enable();

        coralScore.performed += CoralScore;
    }

    private void OnDisable()
    {
        drive.Disable();
        coralScore.Disable();
    }
    
    private void FixedUpdate()
    {
        moveDirection = drive.ReadValue<Vector2>();
        robot.Drive(moveDirection);
    }
    
    private void CoralScore(InputAction.CallbackContext context)
    {
        robotActions.CoralScore();
    }
}