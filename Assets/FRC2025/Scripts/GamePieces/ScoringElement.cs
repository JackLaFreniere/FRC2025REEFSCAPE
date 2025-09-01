using UnityEngine;

namespace FRC2025
{
    public class ScoringElement : MonoBehaviour
    {
        public bool IsScored { get; protected set; } = false;
        public bool ScoredInAuto { get; protected set; } = false;
        public bool CanBeScored { get; protected set; } = true;

        public void SetScored(bool value = true) => IsScored = value;
        public void SetScoredInAuto(bool value = true) => ScoredInAuto = value;
        public void SetCanBeScored(bool value = true) => CanBeScored = value;
    }
}