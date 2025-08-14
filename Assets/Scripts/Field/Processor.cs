using UnityEngine;

public class Processor : ScoreableLocation
{
    private readonly int score = 6;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidScoringObject(other)) return;

        OnScored(other);

        Destroy(other.gameObject);
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
}
