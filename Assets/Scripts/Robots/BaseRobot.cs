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
            if (ToggleCamera.IsRobotCamera)
            {
                RobotCentricDrive(driveInput);
            }
            else
            {
                FieldCentricDrive(driveInput);
            }
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
    /// Drives the robot in a field-centric manner based on the provided input vector.
    /// </summary>
    /// <param name="driveInput">A <see cref="Vector2"/> representing the desired movement direction and magnitude. The X component controls lateral
    /// movement, and the Y component controls forward and backward movement.</param>
    public void FieldCentricDrive(Vector2 driveInput)
    {
        robotRigidbody.AddForce(-driveInput.y * driveSpeed * Time.deltaTime, 0, driveInput.x * driveSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

    /// <summary>
    /// Moves the robot in a direction relative to its local coordinate system based on the provided input.
    /// </summary>
    /// <param name="driveInput">A <see cref="Vector2"/> representing the desired movement direction in the robot's local space. The X component
    /// controls lateral movement, and the Y component controls forward/backward movement.</param>
    public void RobotCentricDrive(Vector2 driveInput)
    {
        Vector3 localInput = new (-driveInput.x, 0, -driveInput.y);
        Vector3 worldDirection = transform.TransformDirection(localInput);

        robotRigidbody.AddForce(driveSpeed * Time.fixedDeltaTime * worldDirection, ForceMode.Force);
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

    /// <summary>
    /// Retrieves the camera component attached to the GameObject named "RobotCamera".
    /// </summary>
    /// <returns>The <see cref="Camera"/> component of the GameObject named "RobotCamera"</returns>
    public static Camera GetRobotCamera()
    {
        return GameObject.Find("RobotCamera").GetComponent<Camera>();
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