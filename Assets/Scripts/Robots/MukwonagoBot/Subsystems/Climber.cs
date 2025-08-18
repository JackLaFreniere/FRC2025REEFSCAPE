using UnityEngine;

public class Climber : MonoBehaviour
{
    [SerializeField] private HingeJoint hinge;

    private readonly float tolerance = SubsystemHelper.defaultAngularTolerance;

    private float targetRotation;

    /// <summary>
    /// Sets the target rotation for the hinge joint, adjusted by the climber offset.
    /// </summary>
    /// <param name="targetAngle">The desired target angle, in degrees, for the hinge joint before applying the climber offset.</param>
    public void SetTargetRotation(float targetAngle)
    {
        targetRotation = -targetAngle;

        SubsystemHelper.UpdateHingeJointSpring(hinge, targetRotation);
    }

    /// <summary>
    /// Determines if the Climber is at its target rotation within a specified tolerance.
    /// </summary>
    /// <returns>If within tolerance</returns>
    public bool IsAtTargetRotation()
    {
        return SubsystemHelper.IsAtRotation(transform.localRotation, targetRotation, tolerance);
    }
}
