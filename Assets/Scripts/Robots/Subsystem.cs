using UnityEngine;

public class Subsystem : MonoBehaviour
{
    public static float defaultAngularTolerance = 5f;
    public static float defaultPositionTolerance = 0.05f;
    
    [Header("Target transform to follow")]
    [SerializeField] private Transform target;

    [Header("Lock position axes (X, Y, Z)")]
    [SerializeField] private bool lockXPos = true;
    [SerializeField] private bool lockYPos = true;
    [SerializeField] private bool lockZPos = true;

    [Header("Lock rotation axes (X, Y, Z)")]
    [SerializeField] private bool lockXRot = true;
    [SerializeField] private bool lockYRot = true;
    [SerializeField] private bool lockZRot = true;

    [Header("Minimum Position Values")]
    [SerializeField] private float minX = float.NegativeInfinity;
    [SerializeField] private float minY = float.NegativeInfinity;
    [SerializeField] private float minZ = float.NegativeInfinity;

    [Header("Offset Position")]
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        FixPosition();
        FixRotation();
    }

    /// <summary>
    /// Adjusts the position of the current object to align with the target's position and rotation, while respecting
    /// axis locking and minimum position constraints.
    /// </summary>
    private void FixPosition()
    {
        Vector3 targetPos = target.position + target.rotation * offset;
        Vector3 newPos = transform.position;

        if (lockXPos) newPos.x = targetPos.x;
        if (lockYPos) newPos.y = targetPos.y;
        if (lockZPos) newPos.z = targetPos.z;

        if (newPos.y < minY) newPos.y = minY;
        if (newPos.x < minX) newPos.x = minX;
        if (newPos.z < minZ) newPos.z = minZ;

        transform.position = newPos;
    }

    /// <summary>
    /// Adjusts the object's rotation to align with the target rotation while preserving freedom of movement along a
    /// specified axis.
    /// </summary>
    private void FixRotation()
    {
        // Determine which axis is free
        Vector3 freeAxis;
        if (!lockXRot) freeAxis = Vector3.right;
        else if (!lockYRot) freeAxis = Vector3.up;
        else if (!lockZRot) freeAxis = Vector3.forward;
        else
        {
            transform.rotation = target.rotation;
            return;
        }

        Quaternion relative = Quaternion.Inverse(target.rotation) * transform.rotation;
        Quaternion twist = ExtractTwistAroundAxis(relative, freeAxis);
        transform.rotation = target.rotation * twist;
    }

    /// <summary>
    /// Extracts the twist component of a quaternion around a specified axis.
    /// </summary>
    /// <param name="q">The input quaternion from which the twist component is extracted.</param>
    /// <param name="axis">The axis around which the twist is calculated. This vector must be non-zero and will be normalized internally.</param>
    /// <returns>A normalized quaternion representing the twist component of the input quaternion around the specified axis.</returns>
    private static Quaternion ExtractTwistAroundAxis(Quaternion q, Vector3 axis)
    {
        axis.Normalize();
        Vector3 qVec = new (q.x, q.y, q.z);
        Vector3 proj = Vector3.Project(qVec, axis);
        Quaternion twist = new (proj.x, proj.y, proj.z, q.w);
        return twist.normalized;
    }

    /// <summary>
    /// Converts a measurement from inches to meters.
    /// </summary>
    /// <param name="i">The measurement in inches to be converted. Must be a non-negative value.</param>
    /// <returns>The equivalent measurement in meters.</returns>
    public static float InchesToMeters(float i)
    {
        return i * 0.0254f;
    }

    /// <summary>
    /// Determines whether the value <paramref name="c"/> is approximately equal to the target value <paramref name="t"/>  within a specified tolerance.
    /// </summary>
    /// <param name="c">The current value to compare.</param>
    /// <param name="t">The target value to compare against.</param>
    /// <param name="e">The tolerance within which the values are considered equal. Defaults to 0.01.</param>
    /// <returns><see langword="true"/> if the absolute difference between <paramref name="c"/> and <paramref name="t"/>  is less
    /// than or equal to <paramref name="e"/>; otherwise, <see langword="false"/>.</returns>
    public static bool IsAtPosition(float c, float t, float e = 0.01f)
    {
        return Mathf.Abs(c - t) <= e;
    }

    /// <summary>
    /// Determines whether the specified quaternion is within a specified tolerance of a target rotation around the X-axis.
    /// </summary>
    /// <param name="c">The quaternion to evaluate.</param>
    /// <param name="t">The target rotation angle, in degrees, around the X-axis.</param>
    /// <param name="e">The tolerance value, in degrees, for determining if the quaternion is at the target rotation. Defaults to 0.01.</param>
    /// <returns><see langword="true"/> if the quaternion is within the specified tolerance of the target rotation; otherwise,
    /// <see langword="false"/>.</returns>
    public static bool IsAtRotation(float c, float t, float e = 0.01f)
    {
        return Mathf.Abs(Quaternion.Angle(Quaternion.Euler(c, 0f, 0f), Quaternion.Euler(t, 0f, 0f))) <= e;
    }

    /// <summary>
    /// Updates the spring settings of a <see cref="HingeJoint"/> to set a new target rotation.
    /// </summary>
    /// <param name="hinge">The <see cref="HingeJoint"/> whose spring settings will be updated. Cannot be <see langword="null"/>.</param>
    /// <param name="targetRotation">The desired target rotation, in degrees, for the hinge joint's spring.</param>
    public static void UpdateHingeJointSpring(HingeJoint hinge, float targetRotation)
    {
        JointSpring spring = hinge.spring;
        spring.targetPosition = targetRotation;
        hinge.spring = spring;
    }
}