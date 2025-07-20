using UnityEngine;

public class Algae : MonoBehaviour
{
    private bool isScored = false;

    /// <summary>
    /// Marks the current instance as scored.
    /// </summary>
    public void Score()
    {
        isScored = true;
    }

    /// <summary>
    /// Determines whether the current instance is marked as scored.
    /// </summary>
    /// <returns><see langword="true"/> if the instance is scored; otherwise, <see langword="false"/>.</returns>
    public bool GetIsScored()
    {
        return isScored;
    }
}
