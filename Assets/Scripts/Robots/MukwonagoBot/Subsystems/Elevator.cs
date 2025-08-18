using UnityEngine;

public class Elevator : MonoBehaviour
{
    private readonly float tolerance = SubsystemHelper.defaultPositionTolerance;

    private Vector3 targetPosition;
    private float elevatorOffset;

    private ConfigurableJoint joint;

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();

        elevatorOffset = transform.localPosition.y;
    }

    private void Update()
    {
        if (transform.localPosition.y < elevatorOffset) transform.localPosition = new Vector3(transform.localPosition.x, elevatorOffset, transform.localPosition.z);
    }

    /// <summary>
    /// Sets the target position of the elevator based on the specified height.
    /// </summary>
    /// <param name="height">The desired height in inches. Must be a positive value.</param>
    public void SetTargetPosition(float height)
    {
        targetPosition = new Vector3(0f, SubsystemHelper.InchesToMeters(height), 0f);

        joint.targetPosition = targetPosition;
    }


    /// <summary>
    /// Determines if the Elevator is at its target position within a specified tolerance.
    /// </summary>
    /// <returns>If within tolerance</returns>
    public bool IsAtTargetPosition()
    {
        return SubsystemHelper.IsAtPosition(transform.localPosition.y, targetPosition.y, tolerance);
    }
}