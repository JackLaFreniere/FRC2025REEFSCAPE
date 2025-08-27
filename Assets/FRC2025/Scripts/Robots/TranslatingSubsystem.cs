using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class TranslatingSubsystem : Subsystem
{
    [Header("Configurable Joint Settings")]
    [SerializeField] protected ConfigurableJoint joint;

    protected readonly float tolerance = defaultPositionTolerance;

    protected Vector3 targetPosition;
    protected Vector3 elevatorOffset;

    /// <summary>
    /// Sets the target position of the joint along the vertical axis.
    /// </summary>
    /// <param name="height">The height, in inches, to set as the target position. This value is converted to meters internally.</param>
    public void SetTargetPosition(float height)
    {
        targetPosition = new Vector3(0f, InchesToMeters(height), 0f);

        joint.targetPosition = targetPosition;
    }


    /// <summary>
    /// Determines if the Elevator is at its target position within a specified tolerance.
    /// </summary>
    /// <returns>If within tolerance</returns>
    public bool IsAtTargetPosition()
    {
        return IsAtPosition(transform.localPosition.y, targetPosition.y, tolerance);
    }
}
