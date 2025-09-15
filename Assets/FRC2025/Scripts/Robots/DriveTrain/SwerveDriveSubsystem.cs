using UnityEngine;

namespace FRC2025
{
    public class SwerveDriveSubsystem : DriveTrainSubsystem
    {
        [Header("Swerve Drive Settings")]
        [SerializeField] private float _driveSpeed = 500f;
        [SerializeField] private float _steerSpeed = 360f; // degrees per second

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

            float forward = Mathf.Clamp(_leftJoystickInput.y, -1f, 1f);
            float strafe = Mathf.Clamp(_leftJoystickInput.x, -1f, 1f);
            float rotate = Mathf.Clamp(_rightJoystickInput.x, -1f, 1f);

            if (_wheelColliders == null || _wheelTransforms == null) return;

            if (!(Mathf.Abs(forward) > 0.01f || Mathf.Abs(strafe) > 0.01f || Mathf.Abs(rotate) > 0.01f))
            {
                for (int i = 0; i < 4; i++)
                {
                    _wheelColliders[i].motorTorque = 0f;
                }

                return;
            }

            Vector2[] targetVectors = new Vector2[4];

            for (int i = 0; i < 4; i++)
            {
                Vector2 forwardVector = GetDriveForwardVector2() * forward;
                Vector2 strafeVector = GetDriveRightVector2() * strafe;
                Vector2 rotateVector = GetDriveRotateVector2(i) * rotate;

                targetVectors[i] = forwardVector + strafeVector + rotateVector;
            }

            // Normalize targetVectors so the largest magnitude is at most 1
            float maxMagnitude = 0f;
            for (int i = 0; i < targetVectors.Length; i++)
            {
                float mag = targetVectors[i].magnitude;
                if (mag > maxMagnitude)
                    maxMagnitude = mag;
            }

            if (maxMagnitude > 1f)
            {
                for (int i = 0; i < targetVectors.Length; i++)
                {
                    targetVectors[i] /= maxMagnitude;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 targetVector = targetVectors[i];
                float targetAngle = Mathf.Atan2(targetVector.x, targetVector.y) * Mathf.Rad2Deg;
                float currentAngle = _wheelTransforms[i].localEulerAngles.y;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float steerStep = _steerSpeed * Time.fixedDeltaTime;
                if (Mathf.Abs(angleDiff) > steerStep)
                {
                    angleDiff = Mathf.Sign(angleDiff) * steerStep;
                }

                _wheelTransforms[i].localEulerAngles += new Vector3(0f, angleDiff, 0f);
                _wheelColliders[i].steerAngle = _wheelTransforms[i].localEulerAngles.y;
                _wheelColliders[i].motorTorque = targetVector.magnitude * _driveSpeed;
            }
        }
        
        private Vector2 GetDriveForwardVector2()
        {
            return new Vector2(0f, 1f);
        }

        private Vector2 GetDriveRightVector2()
        {
            return new Vector2(1f, 0f);
        }

        private Vector2 GetDriveRotateVector2(int index)
        {
            Vector3 wheelPosition = _wheelTransforms[index].localPosition;
            Vector2 initialWheelVector = new Vector2(wheelPosition.x, wheelPosition.z).normalized;
            Vector2 finaleWheelVector = Quaternion.Euler(0f, 0f, -90f) * initialWheelVector;

            return finaleWheelVector;
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