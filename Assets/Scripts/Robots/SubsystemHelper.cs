using UnityEngine;

public class SubsystemHelper : MonoBehaviour
{
    /// <summary>
    /// Converts a measurement in inches to meters.
    /// </summary>
    /// <param name="i">The measurement in inches to be converted. Must be a non-negative value.</param>
    /// <returns>The equivalent measurement in meters.</returns>
    public static float InchesToMeters(float i)
    {
        return i * 0.0254f;
    }

    /// <summary>
    /// Determines if the current position is within a specified tolerance of the target position.
    /// </summary>
    /// <param name="c">Current Position</param>
    /// <param name="t">Target Position</param>
    /// <param name="e">Tolerance</param>
    /// <returns></returns>
    public static bool IsAtPosition(float c, float t, float e = 0.01f)
    {
        return Mathf.Abs(c - t) <= e;
    }

    /// <summary>
    /// Determines if the current rotation is within a specified tolerance of the target rotation.
    /// </summary>
    /// <param name="c">Current Rotation</param>
    /// <param name="t">Target Rotation</param>
    /// <param name="e">Tolerance</param>
    /// <returns></returns>
    public static bool IsAtRotation(Quaternion c, Quaternion t, float e = 0.01f)
    {
        return Quaternion.Angle(c, t) <= e;
    }
}
