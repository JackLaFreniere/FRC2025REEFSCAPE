using UnityEngine;

public class BaseRobot : MonoBehaviour
{
    [SerializeField] private GameObject robot;
    [SerializeField] private float driveSpeed;
    [SerializeField] private float rotateSpeed;
    private Rigidbody robotRigidbody;
    

    private void Start()
    {
        robotRigidbody = robot.GetComponent<Rigidbody>();
    }
    public void Drive(Vector2 driveInput)
    {
        robotRigidbody.AddForce(-driveInput.y * driveSpeed * Time.deltaTime, 0, driveInput.x * driveSpeed * Time.deltaTime, ForceMode.Force);
    }

    public void Rotate(Vector2 rotateInput)
    {
        robotRigidbody.AddTorque(0, rotateInput.x * rotateSpeed * Time.deltaTime, 0, ForceMode.Force);
    }

    public virtual void CoralIntake() { }
    public virtual void AlgaeIntake() { }
    public virtual void CoralScore() { }
    public virtual void NetScore() { }
    public virtual void ProcessorScore() { }
    public virtual void SuperScore() { }
    public virtual void Climb() { }
    public virtual void ClimberDown() { }
    public virtual void ClimberUp() { }
    public virtual void CoralEject() { }
    public virtual void AlgaeEject() { }
}