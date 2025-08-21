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
    /// Code to run when an algae is scored.
    /// </summary>
    /// <param name="other">The Algae that is being scored.</param>
    public override void OnScored(Collider other)
    {
        other.GetComponent<Algae>().Score();

        ScoreManager.AddScore(score, allianceColor);
    }

    /// <summary>
    /// Code to run when an algae is unscored.
    /// </summary>
    /// <param name="other">The Algae that is being unscored.</param>
    public void OnUnscored(Collider other)
    {
        other.GetComponent<Algae>().Unscore();

        ScoreManager.AddScore(-score, allianceColor);
    }
}