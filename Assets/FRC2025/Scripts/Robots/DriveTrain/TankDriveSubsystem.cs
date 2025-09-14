using UnityEngine;

namespace FRC2025
{
    public class TankDriveSubsystem : DriveTrainSubsystem
    {
        [Header("Drive Settings")]
        [SerializeField] protected float _driveSpeed = 10f;

        /// <summary>
        /// Updates the motor torque of the wheel colliders based on joystick input.
        /// </summary>
        /// <remarks>This method calculates the left and right motor speeds using the vertical input from
        /// the left joystick and the horizontal input from the right joystick. The calculated speeds are clamped to
        /// the range [-1, 1] and applied to the corresponding wheel colliders. If the wheel colliders are not
        /// initialized, the method does nothing. In the Unity Editor, this method also updates the wheel colliders'
        /// state for debugging purposes.</remarks>
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            UpdateWheelColliders();
#endif

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