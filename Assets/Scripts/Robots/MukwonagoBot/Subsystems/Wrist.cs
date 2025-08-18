using UnityEngine;

public class Wrist : MonoBehaviour
{
    [SerializeField] private HingeJoint hinge;

    private readonly float tolerance = SubsystemHelper.defaultAngularTolerance;

    private float targetRotation;

    /// <summary>
    /// Sets the target rotation for the hinge joint.
    /// </summary>
    /// <param name="rotation">The desired rotation angle, in degrees, to set as the target for the hinge joint.</param>
    public void SetTargetRotation(float rotation)
    {
        targetRotation = rotation;

        SubsystemHelper.UpdateHingeJointSpring(hinge, targetRotation);
    }

    /// <summary>
    /// Determines if the Wrist is at its target rotation within a specified tolerance.
    /// </summary>
    /// <returns>If within tolerance</returns>
    public bool IsAtTargetRotation()
    {
        return SubsystemHelper.IsAtRotation(transform.localRotation, targetRotation, tolerance);
    }
}
