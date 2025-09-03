using UnityEngine;

namespace FRC2025
{
    [CreateAssetMenu(fileName = "RobotPose", menuName = "Scriptable Objects/RobotPose")]
    public class RobotPose : ScriptableObject
    {
        [System.Serializable]
        public struct SubsystemTarget
        {
            public string subsystemName;
            public float targetValue;
        }

        public SubsystemTarget[] targets;
    }
}