using UnityEngine;

namespace FRC2025 {
    public abstract class Subsystem : MonoBehaviour
    {
        public abstract string SubsystemName { get; }
        public abstract void ApplyTarget(float value);
        public abstract bool IsAtTarget();
    }
}