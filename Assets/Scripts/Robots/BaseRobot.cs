using UnityEngine;

public class BaseRobot : MonoBehaviour
{
    [SerializeField] private GameObject robot;
    [SerializeField] private float speed;
    private Rigidbody robotRigidbody;
    

    private void Start()
    {
        robotRigidbody = robot.GetComponent<Rigidbody>();
    }
    public void Drive(Vector2 input)
    {
        robotRigidbody.AddForce(-input.y * speed * Time.deltaTime, 0, input.x * speed * Time.deltaTime, ForceMode.VelocityChange);
    }

}
