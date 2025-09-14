using UnityEngine;

namespace FRC2025
{
    public class SwerveDriveSubsystem : DriveTrainSubsystem
    {
        [Header("Swerve Drive Settings")]
        [SerializeField] private float _driveSpeed = 500f;
        [SerializeField] private float _steerSpeed = 180f; // degrees per second

        private Transform[] _wheelTransforms;

        private new void Awake()
        {
            base.Awake();

            GetWheelSteerTransforms();
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            UpdateWheelColliders();
#endif

            // Joystick inputs: y = forward/back, x = strafe, z = rotation (if available)
            float forward = Mathf.Clamp(_leftJoystickInput.y, -1f, 1f);
            float strafe = Mathf.Clamp(_leftJoystickInput.x, -1f, 1f);
            float rotate = Mathf.Clamp(_rightJoystickInput.x, -1f, 1f);

            if (_wheelColliders == null || _wheelTransforms == null) return;

            // Only update wheel angles if there is input
            if (Mathf.Abs(forward) > 0.01f || Mathf.Abs(strafe) > 0.01f || Mathf.Abs(rotate) > 0.01f)
            {
                for (int i = 0; i < 4; i++)
                {
                    // Wheel position relative to robot center
                    Vector2 wheelOffset = GetWheelOffset(i);

                    // Calculate desired wheel direction (vector sum of translation and rotation)
                    Vector2 moveDir = new Vector2(strafe, forward);
                    Vector2 rotationDir = new Vector2(-wheelOffset.y, wheelOffset.x) * rotate;
                    Vector2 desiredDir = moveDir + rotationDir;

                    // Calculate the orientation (angle) the wheel should face
                    float desiredAngle = Mathf.Atan2(desiredDir.x, desiredDir.y) * Mathf.Rad2Deg;

                    // Rotate wheel to desired angle, flipping if closer to opposite
                    if (_wheelTransforms[i] != null)
                    {
                        float currentAngle = _wheelTransforms[i].localEulerAngles.y;
                        float angleDelta = Mathf.DeltaAngle(currentAngle, desiredAngle);

                        // Check if flipping 180 is closer
                        float flippedAngle = (desiredAngle + 180f) % 360f;
                        float flippedDelta = Mathf.DeltaAngle(currentAngle, flippedAngle);

                        if (Mathf.Abs(flippedDelta) < Mathf.Abs(angleDelta))
                        {
                            // Flip wheel 180 degrees and steer toward flippedAngle
                            float steerStep = Mathf.Clamp(flippedDelta, -_steerSpeed * Time.fixedDeltaTime, _steerSpeed * Time.fixedDeltaTime);
                            _wheelTransforms[i].localRotation = Quaternion.Euler(0f, currentAngle + steerStep, 90f);
                        }
                        else
                        {
                            // Steer toward desiredAngle normally
                            float steerStep = Mathf.Clamp(angleDelta, -_steerSpeed * Time.fixedDeltaTime, _steerSpeed * Time.fixedDeltaTime);
                            _wheelTransforms[i].localRotation = Quaternion.Euler(0f, currentAngle + steerStep, 90f);
                        }
                    }
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

        public void GetWheelSteerTransforms()
        {
            _wheelTransforms = new Transform[_wheelColliders.Length];

            for (int i = 0; i < _wheelTransforms.Length; i++)
            {
                _wheelTransforms[i] = _wheelColliders[i].transform;
            }
        }
    }
}