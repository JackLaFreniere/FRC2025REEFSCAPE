using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class SwerveDriveGenerator : DriveTrain
    {
#if UNITY_EDITOR
        private void Awake()
        {
            _driveTrainName = "Swerve Drive";
        }
#endif
    }
}