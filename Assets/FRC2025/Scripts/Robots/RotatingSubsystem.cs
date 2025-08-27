using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class RotatingSubsystem : Subsystem
{
    [Header("Hinge Joint Settings")]
    [SerializeField] private HingeJoint hinge;

    [Header("Target Angle Settings")]
    [SerializeField] private bool isInverted = false;
    [SerializeField] private float offsetAngle = 0f;

    protected readonly float tolerance = defaultAngularTolerance;
    protected float targetRotation;

    /// <summary>
    /// Sets the target rotation angle for the hinge joint.
    /// </summary>
    /// <param name="targetAngle">The desired rotation angle, in degrees, to set as the target for the hinge joint.</param>
    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = AdjustTargetAngle(targetAngle);

        Subsystem.UpdateHingeJointSpring(hinge, targetRotation);
    }

    /// <summary>
    /// Determines whether the current rotation is within the specified tolerance of the target rotation.
    /// </summary>
    /// <returns><see langword="true"/> if the current rotation is within the tolerance of the target rotation;  otherwise, <see langword="false"/>.</returns>
    public bool IsAtTargetRotation()
    {
        return Subsystem.IsAtRotation(transform.localRotation.x, targetRotation, tolerance);
    }

    /// <summary>
    /// Calculates the offset value based on the specified target angle.
    /// </summary>
    /// <param name="targetAngle">The target angle, in degrees, for which the offset value is calculated.</param>
    /// <returns>The calculated offset value. By default, this method returns the input <paramref name="targetAngle"/>.</returns>
    protected virtual float AdjustTargetAngle(float targetAngle) { return (isInverted? -targetAngle : targetAngle) + offsetAngle; }
}
