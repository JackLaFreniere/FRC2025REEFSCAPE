using UnityEngine;

namespace FRC2025
{
    public abstract class ScoreableLocation : MonoBehaviour
    {
        [SerializeField] protected AllianceColor allianceColor;
        [SerializeField] protected string scoringElementTag;

        protected virtual bool IsValidScoringObject(Collider other) =>
            other.CompareTag(scoringElementTag) && other.transform.root == other.transform;

        protected abstract void OnScored(Collider other);
        protected abstract void CacheCollider();
        protected abstract void UpdateColliderSettings();
        protected abstract void FixEditor();
        protected virtual void OnTriggerEnter(Collider other) { }
    }
}