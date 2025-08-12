using UnityEngine;

public abstract class ScoreableLocation : MonoBehaviour
{
    public static ScoreManager scoreManager;
    public AllianceColor allianceColor;
    public string scoringElementTag;

    private void Start()
    {
        if (scoreManager == null)
            scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    public abstract void OnScored(Collider other);
    public abstract bool IsValidScoringObject(Collider other);
}
