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
    public void Drive(Vector2 driveInput, Vector2 rotateInput)
    {
        robotRigidbody.AddForce(-driveInput.y * driveSpeed * Time.deltaTime, 0, driveInput.x * driveSpeed * Time.deltaTime, ForceMode.Force);
        robotRigidbody.AddTorque(0, rotateInput.x * rotateSpeed * Time.deltaTime, 0, ForceMode.Force);
    }

    public virtual void CoralIntake()
    {
        Debug.Log("BaseRobot is intaking coral.");
    }

    public virtual void AlgaeIntake()
    {
        Debug.Log("BaseRobot is intaking algae.");
    }

    public virtual void CoralScore()
    {
        Debug.Log("BaseRobot is scoring coral.");
    }

    public virtual void NetScore()
    {
        Debug.Log("BaseRobot is scoring in the net.");
    }

    public virtual void ProcessorScore()
    {
        Debug.Log("BaseRobot is scoring in the processor.");
    }

    public virtual void SuperScore()
    {
        Debug.Log("BaseRobot is performing a super score.");
    }

    public virtual void Climb()
    {
        Debug.Log("BaseRobot is climbing.");
    }

    public virtual void ClimberDown()
    {
        Debug.Log("BaseRobot is moving the climber down.");
    }

    public virtual void ClimberUp()
    {
        Debug.Log("BaseRobot is moving the climber up.");
    }

    public virtual void CoralEject()
    {
        Debug.Log("BaseRobot is ejecting coral.");
    }

    public virtual void AlgaeEject()
    {
        Debug.Log("BaseRobot is ejecting algae.");
    }
}