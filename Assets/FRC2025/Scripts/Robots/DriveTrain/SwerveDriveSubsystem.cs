using UnityEngine;

namespace FRC2025
{
    public class SwerveDriveSubsystem : DriveTrainSubsystem
    {
        [Header("Swerve Drive Settings")]
        [SerializeField] private float _driveSpeed = 500f;
        [SerializeField] private float _steerSpeed = 180f; // degrees per second

        // Each wheel's steering transform (should be set up in the generator)
        [SerializeField] private Transform[] _steerTransforms = new Transform[4];

        private void FixedUpdate()
        {
            // Joystick inputs: y = forward/back, x = strafe, z = rotation (if available)
            float forward = Mathf.Clamp(_leftJoystickInput.y, -1f, 1f);
            float strafe = Mathf.Clamp(_leftJoystickInput.x, -1f, 1f);
            float rotate = Mathf.Clamp(_rightJoystickInput.x, -1f, 1f);

            if (_wheelColliders == null || _steerTransforms == null || _steerTransforms.Length != 4)
                return;

            // Calculate desired wheel angles and speeds for swerve
            for (int i = 0; i < 4; i++)
            {
                // Wheel position relative to robot center
                Vector2 wheelOffset = GetWheelOffset(i);

                // Calculate desired wheel direction (vector sum of translation and rotation)
                Vector2 moveDir = new Vector2(strafe, forward);
                Vector2 rotationDir = new Vector2(-wheelOffset.y, wheelOffset.x) * rotate;
                Vector2 desiredDir = moveDir + rotationDir;

                float desiredAngle = Mathf.Atan2(desiredDir.x, desiredDir.y) * Mathf.Rad2Deg;
                float desiredSpeed = desiredDir.magnitude * _driveSpeed;

                // Rotate wheel to desired angle
                if (_steerTransforms[i] != null)
                {
                    float currentAngle = _steerTransforms[i].localEulerAngles.y;
                    float angleDelta = Mathf.DeltaAngle(currentAngle, desiredAngle);
                    float steerStep = Mathf.Clamp(angleDelta, -_steerSpeed * Time.fixedDeltaTime, _steerSpeed * Time.fixedDeltaTime);
                    _steerTransforms[i].localRotation = Quaternion.Euler(0f, currentAngle + steerStep, 0f);
                }

                // Apply drive force in the wheel's forward direction
                if (_wheelColliders[i] != null)
                {
                    _wheelColliders[i].motorTorque = desiredSpeed;
                }
            }
        }

        // Helper: Returns wheel offset from robot center for swerve math
        private Vector2 GetWheelOffset(int index)
        {
            // Assumes order: 0=FL, 1=FR, 2=BL, 3=BR
            float x = (index % 2 == 0) ? -1f : 1f; // Left/Right
            float y = (index < 2) ? 1f : -1f;      // Front/Back
            return new Vector2(x, y);
        }

        public void SetSteerTransforms(Transform[] transforms)
        {
            _steerTransforms = transforms;
        }
    }
}