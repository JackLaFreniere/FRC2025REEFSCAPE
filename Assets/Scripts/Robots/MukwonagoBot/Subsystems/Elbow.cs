using UnityEngine;

public class Elbow : MonoBehaviour
{
    [SerializeField] private HingeJoint hinge;

    private readonly float tolerance = SubsystemHelper.defaultAngularTolerance;

    private const float elbowOffset = 90f;
    private float targetRotation;

    /// <summary>
    /// Sets the target rotation for the hinge joint, adjusted by the elbow offset.
    /// </summary>
    /// <param name="targetAngle">The desired target angle, in degrees, for the hinge joint before applying the elbow offset.</param>
    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = targetAngle - elbowOffset;

        SubsystemHelper.UpdateHingeJointSpring(hinge, targetRotation);
    }

    /// <summary>
    /// Determines if the Elbow is at its target rotation within a specified tolerance.
    /// </summary>
    /// <returns>If within tolerance</returns>
    public bool IsAtTargetRotation()
    {
        return SubsystemHelper.IsAtRotation(transform.localRotation, targetRotation, tolerance);
    }
}
