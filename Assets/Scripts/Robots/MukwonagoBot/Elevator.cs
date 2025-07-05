using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Elevator : MonoBehaviour
{
    private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private float forceMultiplier;
    private readonly float elevatorOffset = 0.104692f;
    private Rigidbody rb;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //initialRotation = transform.localRotation;
        //initialPosition = transform.localPosition;
    }

    private void Update()
    {
        //transform.localRotation = initialRotation;
        //transform.localPosition = new Vector3(initialPosition.x, transform.localPosition.y, initialPosition.z);
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * forceMultiplier));
    }

    public void SetTargetPosition(float position)
    {
        targetPosition = new Vector3(0f, InchesToMeters(position) + elevatorOffset, 0.036309f);
    }

    public float InchesToMeters(float inches)
    {
        return inches * 0.0254f; // Convert inches to meters
    }
}
