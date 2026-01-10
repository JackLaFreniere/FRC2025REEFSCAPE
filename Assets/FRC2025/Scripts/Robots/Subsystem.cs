using UnityEngine;

namespace FRC2025 {
    public abstract class Subsystem : MonoBehaviour
    {
        protected BaseRobot _baseRobot;
        public abstract void ApplyTarget(float value);
        public abstract bool IsAtTarget();
        
        private void Awake()
        {
            _baseRobot = GetComponentInParent<BaseRobot>();
            if (_baseRobot == null) {
                Debug.LogError("Subsystem must be a child of a BaseRobot");
            }
        }
    }
}