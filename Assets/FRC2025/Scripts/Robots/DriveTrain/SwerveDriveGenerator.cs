using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class SwerveDriveGenerator : DriveTrain
    {
        private void Awake()
        {
            _driveTrainName = "Swerve Drive";
        }
    }
}