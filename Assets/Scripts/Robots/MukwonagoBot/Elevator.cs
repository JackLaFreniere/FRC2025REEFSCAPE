using UnityEngine;

public class Elevator : MonoBehaviour
{
    private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private float movementSpeed = 3f;
    private readonly float elevatorOffset = 0.104692f;

    private void FixedUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.fixedDeltaTime * movementSpeed);
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
