using UnityEngine;

public class SubsystemHelper : MonoBehaviour
{
    public static float defaultAngularTolerance = 5f;
    public static float defaultPositionTolerance = 0.05f;

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
    /// Determines whether the current rotation is approximately equal to the target rotation within a specified tolerance.
    /// </summary>
    /// <param name="c">The current rotation represented as a <see cref="Quaternion"/>.</param>
    /// <param name="t">The target rotation represented as a <see cref="Quaternion"/>.</param>
    /// <param name="e">The tolerance value, expressed in degrees, within which the two rotations are considered equal. Defaults to 0.01.</param>
    /// <returns><see langword="true"/> if the angle between the current and target rotations is less than or equal to the
    /// specified tolerance; otherwise, <see langword="false"/>.</returns>
    public static bool IsAtRotation(Quaternion c, Quaternion t, float e = 0.01f)
    {
        return Quaternion.Angle(c, t) <= e;
    }

    /// <summary>
    /// Determines whether the specified quaternion is within a specified tolerance of a target rotation around the X-axis.
    /// </summary>
    /// <param name="c">The quaternion to evaluate.</param>
    /// <param name="t">The target rotation angle, in degrees, around the X-axis.</param>
    /// <param name="e">The tolerance value, in degrees, for determining if the quaternion is at the target rotation. Defaults to 0.01.</param>
    /// <returns><see langword="true"/> if the quaternion is within the specified tolerance of the target rotation; otherwise,
    /// <see langword="false"/>.</returns>
    public static bool IsAtRotation(Quaternion c, float t, float e = 0.01f)
    {
        return Mathf.Abs(Quaternion.Angle(c, Quaternion.Euler(t, 0f, 0f))) <= e;
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