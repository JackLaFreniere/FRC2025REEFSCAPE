using UnityEngine;
using UnityEngine.InputSystem;

public class BaseRobot : MonoBehaviour
{
    public AllianceColor allianceColor = AllianceColor.Blue;
    public static bool hasCoral = false;
    public static bool hasAlgae = false;

    [Header("Rotation Settings")]
    [SerializeField] private float maxAngularSpeed = 180f; // degrees/sec
    [SerializeField] private float rotationAcceleration = 720f; // degrees/sec^2
    [SerializeField] private float rotationDeceleration = 1080f; // degrees/sec^2
    [SerializeField] private float rotationStickDeadBand = 0.05f; // Deadband for rotation input
    private float currentAngularVelocity = 0f;

    [Header("Drive Settings")]
    [SerializeField] private float maxDriveSpeed = 5f;          // meters per second
    [SerializeField] private float driveAcceleration = 10f;     // meters per second^2
    [SerializeField] private float driveDeceleration = 15f;     // meters per second^2
    [SerializeField] private float driveStickDeadBand = 0.1f; // Deadband for drive input
    private Vector3 currentVelocity = Vector3.zero;

    public static Collider coral = null;
    public static Collider algae = null;

    private GameObject robot;
    [SerializeField] private Rigidbody robotRigidbody;
    private RobotInfo robotInfo;

    private InputAction drive;
    private InputAction rotate;

    public static StateMachine stateMachine;
    public PlayerState stowState;
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
        // Resets the game pieces between instances
        hasCoral = false;
        hasAlgae = false;
        
        coral = null;
        algae = null;

        robot = GameObject.Find("Drive_Base");
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
        // Gets the robot's position from Transform, and rotation from RobotInfo.spawnEuler
        transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
        Vector3 startEuler = robotInfo.spawnEuler;

        Vector3 targetPosition = new (position.x, robotInfo.spawnPosition.y, position.z);
        Quaternion targetRotation = Quaternion.Euler(startEuler.x, rotation.eulerAngles.y, startEuler.z);

        transform.SetPositionAndRotation(targetPosition, targetRotation);
    }

    /// <summary>
    /// Drives the robot based on the specified input vector, adjusting its velocity and direction.
    /// </summary>
    /// <param name="driveInput">A 2D vector representing the input for driving. The X component controls lateral movement,  and the Y component
    /// controls forward and backward movement. Values should typically range between -1 and 1 for each component.</param>
    public void Drive(Vector2 driveInput)
    {
        // Gets the user inputs and accomodates for robot/field centric driving
        Vector3 input = new(-driveInput.y, 0f, driveInput.x);
        Vector3 desiredDirection = GetDirection(input);

        // Clamps the input and create a Vector3 to represent a properly scaled target velocity
        float inputMagnitude = Mathf.Clamp01(driveInput.magnitude);
        Vector3 targetVelocity = desiredDirection * (inputMagnitude * maxDriveSpeed);

        // Uses a deadband to determine if the robot should accelerate or decelerate, and then gets a velocity calculated form the acceleration
        float accel = inputMagnitude > driveStickDeadBand ? driveAcceleration : driveDeceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * Time.fixedDeltaTime);

        robotRigidbody.linearVelocity = new (currentVelocity.x, 0f, currentVelocity.z);
    }

    public Vector3 GetDirection(Vector3 input)
    {
        if (ToggleCamera.IsRobotCamera) return Quaternion.Euler(0f, -90f, 0f) * robot.transform.TransformDirection(input).normalized;
        return input.normalized;
    }

    /// <summary>
    /// Rotates the robot based on the specified input.
    /// </summary>
    /// <param name="rotateInput">A <see cref="Vector2"/> representing the rotation input. The X component determines the rotation speed, where
    /// positive values rotate clockwise and negative values rotate counterclockwise. The Y component is ignored.</param>
    public void Rotate(Vector2 rotateInput)
    {
        float input = rotateInput.x;
        float targetSpeed = input * maxAngularSpeed;

        // Accelerate or decelerate toward target
        float accel = Mathf.Abs(input) > rotationStickDeadBand ? rotationAcceleration : rotationDeceleration;
        currentAngularVelocity = Mathf.MoveTowards(currentAngularVelocity, targetSpeed, accel * Time.fixedDeltaTime);

        // Apply rotation as torque (convert to radians)
        float angularVelocityRad = currentAngularVelocity * Mathf.Deg2Rad;
        robotRigidbody.angularVelocity = new Vector3(0f, angularVelocityRad, 0f);
    }

    /// <summary>
    /// Sets the Coral as scored and removes it from the robot's manipulator.
    /// </summary>
    public static void RemoveCoral()
    {
        coral.GetComponent<Coral>().SetScore();

        coral = null;
        hasCoral = false;
    }

    /// <summary>
    /// Sets the Algae as scored and removes it from the robot's manipulator.
    /// </summary>
    public static void RemoveAlgae()
    {
        algae.GetComponent<Algae>().Score();

        algae = null;
        hasAlgae = false;
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
    public virtual void Stow() { }
    public virtual void TogglePreset() { }
}