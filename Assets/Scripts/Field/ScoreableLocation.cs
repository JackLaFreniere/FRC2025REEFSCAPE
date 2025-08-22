using UnityEngine;

public abstract class ScoreableLocation : MonoBehaviour
{
    public AllianceColor allianceColor;
    public string scoringElementTag;

    public abstract void OnScored(Collider other);
    public virtual bool IsValidScoringObject(Collider other)
    {
        return other.CompareTag(scoringElementTag) && other.transform.root == other.transform;
    }
}
