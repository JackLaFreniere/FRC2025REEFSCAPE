using UnityEngine;
using UnityEngine.InputSystem;

public class BaseRobot : MonoBehaviour
{
    //[SerializeField] private GameObject robot;
    private GameObject robot;
    [SerializeField] private float driveSpeed;
    [SerializeField] private float rotateSpeed;
    private Rigidbody robotRigidbody;

    private InputAction drive;
    private InputAction rotate;

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
        }
    }

    public void Drive(Vector2 driveInput)
    {
        robotRigidbody.AddForce(-driveInput.y * driveSpeed * Time.deltaTime, 0, driveInput.x * driveSpeed * Time.deltaTime, ForceMode.Force);
    }

    public void Rotate(Vector2 rotateInput)
    {
        robotRigidbody.AddTorque(0, rotateInput.x * rotateSpeed * Time.deltaTime, 0, ForceMode.Force);
    }

    public void SetDrive(InputAction drive)
    {
        this.drive = drive;
    }
    
    public void SetRotate(InputAction rotate)
    {
        this.rotate = rotate;
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
    public virtual void Idle() { }
}