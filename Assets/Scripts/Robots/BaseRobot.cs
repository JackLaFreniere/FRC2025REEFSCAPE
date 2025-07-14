using UnityEngine;
using UnityEngine.InputSystem;

public class BaseRobot : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float driveSpeed;
    [SerializeField] private float rotateSpeed;

    private GameObject robot;
    private Rigidbody robotRigidbody;
    private RobotInfo robotInfo;

    private InputAction drive;
    private InputAction rotate;

    public StateMachine stateMachine;
    public PlayerState idleState;
    public PlayerState algaeIntakeState;
    public PlayerState coralIntakeState;
    public PlayerState coralScoreState;
    public PlayerState netScoreState;
    public PlayerState processorScoreState;
    public PlayerState climberDownState;
    public PlayerState climberUpState;
    public PlayerState coralEjectState;
    public PlayerState algaeEjectState;
    public PlayerState confirmCoralScore;

    protected virtual void Start()
    {
        robot = this.gameObject;
        robotRigidbody = robot.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (drive != null && rotate != null)
        {
            Vector2 driveInput = drive.ReadValue<Vector2>();
            Vector2 rotateInput = rotate.ReadValue<Vector2>();
            Drive(driveInput);
            Rotate(rotateInput);

            FreezeTransform();
        }
    }

    /// <summary>
    /// Locks the robot's position along the y-axis and its rotation around the x and z axes.
    /// </summary>
    private void FreezeTransform()
    {
        transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

        Vector3 targetPosition = new (position.x, robotInfo.spawnPosition.y, position.z);
        Quaternion targetRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

        transform.SetPositionAndRotation(targetPosition, targetRotation);
    }

    /// <summary>
    /// Moves the robot based on the specified input vector.
    /// </summary>
    /// <param name="driveInput">A <see cref="Vector2"/> representing the movement input, where the Y component controls forward and backward
    /// movement,  and the X component controls lateral movement.</param>
    public void Drive(Vector2 driveInput)
    {
        robotRigidbody.AddForce(-driveInput.y * driveSpeed * Time.deltaTime, 0, driveInput.x * driveSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

    /// <summary>
    /// Rotates the robot based on the specified input vector.
    /// </summary>
    /// <param name="rotateInput">A <see cref="Vector2"/> representing the rotation input. The <c>x</c> component determines the rotation speed
    /// and direction.</param>
    public void Rotate(Vector2 rotateInput)
    {
        robotRigidbody.AddTorque(0, rotateInput.x * rotateSpeed * Time.fixedDeltaTime, 0, ForceMode.Force);
    }

    /// <summary>
    /// Sets the input action used to control the drive mechanism.
    /// </summary>
    /// <param name="drive">The input action that defines the drive behavior. Cannot be null.</param>
    public void SetDrive(InputAction drive)
    {
        this.drive = drive;
    }
    
    /// <summary>
    /// Sets the input action used to control rotation.
    /// </summary>
    /// <param name="rotate">The input action that defines rotation behavior. Cannot be null.</param>
    public void SetRotate(InputAction rotate)
    {
        this.rotate = rotate;
    }

    /// <summary>
    /// Updates the robot's information with the specified data.
    /// </summary>
    /// <param name="robotInfo">The new robot information to set. This parameter cannot be null.</param>
    public void SetRobotInfo(RobotInfo robotInfo)
    {
        this.robotInfo = robotInfo;
    }

    public virtual void CoralIntake() { }
    public virtual void AlgaeIntake() { }
    public virtual void CoralScore() { }
    public virtual void NetScore() { }
    public virtual void ProcessorScore() { }
    public virtual void SuperScore() { }
    public virtual void ClimberDown() { }
    public virtual void ClimberUp() { }
    public virtual void CoralEject() { }
    public virtual void AlgaeEject() { }
    public virtual void ConfirmCoralScore() { }
    public virtual void Idle() { }
}