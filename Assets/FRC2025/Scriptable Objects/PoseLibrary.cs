using UnityEngine;

namespace FRC2025 {
    [CreateAssetMenu(fileName = "PoseLibrary", menuName = "Scriptable Objects/PoseLibrary")]
    public class PoseLibrary : ScriptableObject
    {
        public RobotPose stow;
        public RobotPose scoreL1;
        public RobotPose scoreL2;
        public RobotPose scoreL3;
        public RobotPose scoreL4;
        public RobotPose lowAlgaeIntake;
        public RobotPose highAlgaeIntake;
        public RobotPose ejectCoral;
        public RobotPose ejectAlgae;
    }
}