using UnityEngine;

public class Shoulder : MonoBehaviour
{
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 10f;
    private readonly float shoulderOffset = 90f;

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = Quaternion.Euler(-targetAngle + shoulderOffset, 0f, 0f);
    }
}
