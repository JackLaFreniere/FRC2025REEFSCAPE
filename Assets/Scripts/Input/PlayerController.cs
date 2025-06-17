using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public BaseRobot robot;
    private MukwonagoBotInput playerControls;
    private BaseRobot robotActions;

    private Vector2 moveDirection;
    private Vector2 rotateDirection;

    private InputAction drive;
    private InputAction rotate;
    private InputAction algaeIntake;
    private InputAction coralIntake;
    private InputAction coralScore;
    private InputAction netScore;
    private InputAction processorScore;
    private InputAction superScore;
    private InputAction climberDown;
    private InputAction climberUp;
    private InputAction coralEject;
    private InputAction algaeEject;

    private void Start()
    {
        robotActions = robot.GetComponent<BaseRobot>();
    }

    private void Awake()
    {
        playerControls = new MukwonagoBotInput();
    }

    private InputAction BindInput(ref InputAction actionField, InputAction inputAction)
    {
        actionField = inputAction;
        return actionField;
    }

    private InputAction BindAction(InputAction action, System.Action<InputAction.CallbackContext> callback)
    {
        action.performed += callback;
        return action;
    }


    private void OnEnable()
    {
        playerControls.Player.Enable();

        //Initializes the InputActions from the InputActions asset
        BindInput(ref drive, playerControls.Player.Drive);
        BindInput(ref rotate, playerControls.Player.Rotate);
        BindInput(ref algaeIntake, playerControls.Player.AlgaeIntake);
        BindInput(ref coralIntake, playerControls.Player.CoralIntake);
        BindInput(ref coralScore, playerControls.Player.CoralScore);
        BindInput(ref netScore, playerControls.Player.NetScore);
        BindInput(ref processorScore, playerControls.Player.ProcessorScore);
        BindInput(ref superScore, playerControls.Player.SuperScore);
        BindInput(ref climberDown, playerControls.Player.ClimberDown);
        BindInput(ref climberUp, playerControls.Player.ClimberUp);
        BindInput(ref coralEject, playerControls.Player.CoralEject);
        BindInput(ref algaeEject, playerControls.Player.AlgaeEject);

        //Subscribes to the performed event for each InputAction
        BindAction(algaeIntake, AlgaeIntake);
        BindAction(coralIntake, CoralIntake);
        BindAction(coralScore, CoralScore);
        BindAction(netScore, NetScore);
        BindAction(processorScore, ProcessorScore);
        BindAction(superScore, SuperScore);
        BindAction(climberDown, ClimberDown);
        BindAction(climberUp, ClimberUp);
        BindAction(coralEject, CoralEject);
        BindAction(algaeEject, AlgaeEject);
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void FixedUpdate()
    {
        moveDirection = drive.ReadValue<Vector2>();
        rotateDirection = rotate.ReadValue<Vector2>();
        robot.Drive(moveDirection, rotateDirection);
    }

    private void AlgaeIntake(InputAction.CallbackContext context) { robotActions.AlgaeIntake(); }
    private void CoralIntake(InputAction.CallbackContext context) { robotActions.CoralIntake(); }
    private void CoralScore(InputAction.CallbackContext context) { robotActions.CoralScore(); }
    private void NetScore(InputAction.CallbackContext context) { robotActions.NetScore(); }
    private void ProcessorScore(InputAction.CallbackContext context) { robotActions.ProcessorScore(); }
    private void SuperScore(InputAction.CallbackContext context) { robotActions.SuperScore(); }
    private void ClimberDown(InputAction.CallbackContext context) { robot.ClimberDown(); }
    private void ClimberUp(InputAction.CallbackContext context) { robot.ClimberUp(); }
    private void CoralEject(InputAction.CallbackContext context) { robot.CoralEject(); }
    private void AlgaeEject(InputAction.CallbackContext context) { robot.AlgaeEject(); }
}