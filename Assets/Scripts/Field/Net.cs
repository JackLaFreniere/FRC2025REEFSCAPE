using System.Collections.Generic;
using UnityEngine;

public class Net : ScoreableLocation
{
    private readonly int score = 4;
    private readonly HashSet<Collider> scoredAlgae = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidScoringObject(other)) return;

        scoredAlgae.Add(other);
        OnScored(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsValidScoringObject(other)) return;

        if (scoredAlgae.Add(other)) OnScored(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidScoringObject(other)) return;

        if (scoredAlgae.Remove(other)) OnUnscored(other);
    }

    /// <summary>
    /// Checks if is valid scoring object.
    /// </summary>
    /// <param name="other">The collider to check.</param>
    /// <returns>If the collider was an acceptable Algae.</returns>
    public override bool IsValidScoringObject(Collider other)
    {
        return other.CompareTag(scoringElementTag) && other.transform.root == other.transform;
    }

    /// <summary>
    /// Code to run when an algae is scored.
    /// </summary>
    /// <param name="other">The Algae that is being scored.</param>
    public override void OnScored(Collider other)
    {
        other.GetComponent<Algae>().Score();

        scoreManager.AddScore(score, allianceColor);
    }

    /// <summary>
    /// Code to run when an algae is unscored.
    /// </summary>
    /// <param name="other">The Algae that is being unscored.</param>
    public void OnUnscored(Collider other)
    {
        other.GetComponent<Algae>().Unscore();

        scoreManager.AddScore(-score, allianceColor);
    }
}