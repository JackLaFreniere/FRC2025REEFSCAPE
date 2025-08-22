using UnityEngine;

public class Coral : MonoBehaviour
{
    private bool isScored = false;
    private bool scoredInAuto = false;

    /// <summary>
    /// Marks the current instance as scored.
    /// </summary>
    public void SetScore()
    {
        isScored = true;
    }

    /// <summary>
    /// Marks the current instance as having scored during the autonomous period.
    /// </summary>
    public void SetScoredInAuto(bool isScored)
    {
        scoredInAuto = isScored;
    }

    /// <summary>
    /// Determines whether the current instance is marked as scored.
    /// </summary>
    /// <returns><see langword="true"/> if the instance is scored; otherwise, <see langword="false"/>.</returns>
    public bool GetIsScored()
    {
        return isScored;
    }

    /// <summary>
    /// Gets a value indicating whether the scoring occurred during the autonomous phase.
    /// </summary>
    public bool GetScoredInAuto()
    {
        return scoredInAuto;
    }
}