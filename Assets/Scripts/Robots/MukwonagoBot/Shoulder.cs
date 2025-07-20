using UnityEngine;

public class Shoulder : MonoBehaviour
{
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 10f;
    private const float shoulderOffset = 90f;
    private const float tolerance = 5f;

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = Quaternion.Euler(-targetAngle + shoulderOffset, 0f, 0f);
    }

    /// <summary>
    /// Determines if the Shoulder is at its target rotation within a specified tolerance.
    /// </summary>
    /// <returns>If within tolerance</returns>
    public bool IsAtTargetRotation()
    {
        return SubsystemHelper.IsAtRotation(transform.localRotation, targetRotation, tolerance);
    }
}