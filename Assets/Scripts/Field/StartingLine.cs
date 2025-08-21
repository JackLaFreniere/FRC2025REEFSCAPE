using UnityEngine;

public class StartingLine : MonoBehaviour
{
    [Header("Robot Information")]
    [SerializeField] private string robotTag;

    public AllianceColor allianceColor;

    private const int mobilityPoints = 3;
    private int mobilityPointsAwarded = 0;

    private bool hasAutoEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(robotTag)) return;

        if (!Timer.IsAuto() || hasAutoEnded) return;

        if (mobilityPointsAwarded == 0) return;

        ScoreManager.AddScore(-mobilityPoints, allianceColor);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(robotTag)) return;

        if (!Timer.IsAuto() || hasAutoEnded) return;
        
        ScoreManager.AddScore(mobilityPoints, allianceColor);
        mobilityPointsAwarded += mobilityPoints;
    }

    private void Update()
    {
        if (!Timer.IsAuto() && !hasAutoEnded)
        {
            hasAutoEnded = true;
        }
    }
}