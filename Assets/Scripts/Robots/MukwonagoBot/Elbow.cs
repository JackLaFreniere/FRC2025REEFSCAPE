using UnityEngine;

public class Elbow : MonoBehaviour
{
    private Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
    [SerializeField] private float rotationSpeed = 10f;
    private readonly float elbowOffset = 90f;

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = Quaternion.Euler(targetAngle - elbowOffset, 0f, 0f);
    }
}
