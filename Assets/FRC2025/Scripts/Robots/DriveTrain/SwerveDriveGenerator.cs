using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class SwerveDriveGenerator : DriveTrainGenerator<SwerveDriveSubsystem>
    {
#if UNITY_EDITOR
        private void Awake()
        {
            _driveTrainName = "Swerve Drive";
        }
#endif
    }
}