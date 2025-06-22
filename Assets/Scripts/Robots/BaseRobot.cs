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

    public virtual void CoralIntakePerformed() { }
    public virtual void CoralIntakeCanceled() { }
    public virtual void AlgaeIntakePerformed() { }
    public virtual void AlgaeIntakeCanceled() { }
    public virtual void CoralScorePerformed() { }
    public virtual void CoralScoreCanceled() { }
    public virtual void NetScorePerformed() { }
    public virtual void NetScoreCanceled() { }
    public virtual void ProcessorScorePerformed() { }
    public virtual void ProcessorScoreCanceled() { }
    public virtual void SuperScorePerformed() { }
    public virtual void SuperScoreCanceled() { }
    public virtual void ClimberDownPerformed() { }
    public virtual void ClimberDownCanceled() { }
    public virtual void ClimberUpPerformed() { }
    public virtual void ClimberUpCanceled() { }
    public virtual void CoralEjectPerformed() { }
    public virtual void CoralEjectCanceled() { }
    public virtual void AlgaeEjectPerformed() { }
    public virtual void AlgaeEjectCanceled() { }
}