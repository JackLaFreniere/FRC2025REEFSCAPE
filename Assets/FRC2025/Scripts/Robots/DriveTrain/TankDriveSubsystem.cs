using UnityEngine;

namespace FRC2025
{
    public class TankDriveSubsystem : DriveTrainSubsystem
    {
        [Header("Drive Settings")]
        [SerializeField] private float _driveSpeed;

        private void FixedUpdate()
        {
            float leftSpeed = Mathf.Clamp(_leftJoystickInput.y + _rightJoystickInput.x, -1f, 1f);
            float rightSpeed = Mathf.Clamp(_leftJoystickInput.y - _rightJoystickInput.x, -1f, 1f);

            if (_wheelColliders != null)
            {
                for (int i = 0; i < _wheelColliders.Length / 2; i++)
                {
                    _wheelColliders[i].motorTorque = leftSpeed * _driveSpeed;
                    _wheelColliders[i + 3].motorTorque = rightSpeed * _driveSpeed;
                }
            }
        }
    }
}