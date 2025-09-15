using UnityEngine;
using UnityEngine.UIElements;

namespace FRC2025
{
    public class SwerveDriveSubsystem : DriveTrainSubsystem
    {
        [Header("Swerve Drive Settings")]
        [SerializeField] private bool _isFieldCentric = true;
        [SerializeField] private float _driveSpeed = 10f;
        [SerializeField] private float _steerSpeed = 720f; // degrees per second

        [Header("Deadband Settings")]
        [SerializeField, Range(0f, 1f)] private float _driveDeadband = 0.01f;
        [SerializeField, Range(0f, 1f)] private float _rotateDeadband = 0.01f;

        private Transform[] _wheelTransforms;

        /// <summary>
        /// Initializes the component and prepares the wheel steer transforms.
        /// </summary>
        /// <remarks>This method overrides the base <c>Awake</c> method to perform additional setup
        /// specific to this component. It ensures that the wheel steer transforms are retrieved and ready for
        /// use.</remarks>
        private new void Awake()
        {
            base.Awake();

            GetWheelSteerTransforms();
        }

        /// <summary>
        /// Updates the robot's wheel colliders and transforms to apply movement based on the current drive inputs.
        /// </summary>
        /// <remarks>This method is called at a fixed time interval and is responsible for processing
        /// movement inputs and applying them to the robot's wheels. It supports both field-centric and robot-centric
        /// drive modes. If the robot is within the deadband for movement inputs, the wheels are stopped.</remarks>
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            UpdateWheelColliders();
#endif
            float forward = 0, strafe = 0, rotate = 0;

            if (_isFieldCentric)
                UpdateFieldCentricDriveInputs(ref forward, ref strafe, ref rotate);
            else
                UpdateRobotCentricDriveInputs(ref forward, ref strafe, ref rotate);
            
            if (_wheelColliders == null || _wheelTransforms == null) return;

            if (IsInDeadBand(forward, strafe, rotate))
            {
                for (int i = 0; i < 4; i++)
                {
                    _wheelColliders[i].motorTorque = 0f;
                }

                return;
            }

            Vector2[] targetVectors = new Vector2[4];

            targetVectors = GetTargetVectors(targetVectors, forward, strafe, rotate);
            targetVectors = NormalizeVectors(targetVectors);

            UpdateAllWheels(targetVectors);
        }
        
        /// <summary>
        /// Calculates the forward direction vector for driving in a 2D space.
        /// </summary>
        /// <returns>A <see cref="Vector2"/> representing the forward direction, with a value of (0, 1).</returns>
        private Vector2 GetDriveForwardVector2()
        {
            return new Vector2(0f, 1f);
        }

        /// <summary>
        /// Returns a unit vector representing the right direction in a 2D coordinate system.
        /// </summary>
        /// <returns>A <see cref="Vector2"/> with an X value of 1 and a Y value of 0.</returns>
        private Vector2 GetDriveRightVector2()
        {
            return new Vector2(1f, 0f);
        }

        /// <summary>
        /// Calculates a 2D vector representing the rotation of a wheel at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the wheel to calculate the rotation vector for.</param>
        /// <returns>A <see cref="Vector2"/> representing the rotated direction of the wheel.</returns>
        private Vector2 GetDriveRotateVector2(int index)
        {
            Vector3 wheelPosition = _wheelTransforms[index].localPosition;
            Vector2 initialWheelVector = new Vector2(wheelPosition.x, wheelPosition.z).normalized;
            Vector2 finaleWheelVector = Quaternion.Euler(0f, 0f, -90f) * initialWheelVector;

            return finaleWheelVector;
        }

        /// <summary>
        /// Populates the internal array of wheel transforms with the transforms of the associated wheel colliders.
        /// </summary>
        /// <remarks>This method initializes and updates the internal array of transforms to match the
        /// transforms of the wheel colliders. It is typically used to synchronize the visual representation of the
        /// wheels with their physical counterparts.</remarks>
        public void GetWheelSteerTransforms()
        {
            _wheelTransforms = new Transform[_wheelColliders.Length];

            for (int i = 0; i < _wheelTransforms.Length; i++)
            {
                _wheelTransforms[i] = _wheelColliders[i].transform;
            }
        }

        /// <summary>
        /// Updates the forward, strafe, and rotate inputs to align with a field-centric coordinate system based on the
        /// robot's current orientation.
        /// </summary>
        /// <remarks>This method adjusts the forward and strafe inputs by rotating them according to the
        /// robot's current yaw angle. The rotation input may also be updated to ensure proper handling of rotational
        /// movement.</remarks>
        /// <param name="forward">A reference to the forward input value, which will be updated to reflect the field-centric forward
        /// direction.</param>
        /// <param name="strafe">A reference to the strafe input value, which will be updated to reflect the field-centric strafe direction.</param>
        /// <param name="rotate">A reference to the rotate input value, which may be modified to adjust the robot's rotation.</param>
        private void UpdateFieldCentricDriveInputs(ref float forward, ref float strafe, ref float rotate)
        {
            // Get the robot's yaw angle in degrees
            float robotYaw = transform.eulerAngles.y + 90f;
            float cos = Mathf.Cos(robotYaw * Mathf.Deg2Rad);
            float sin = Mathf.Sin(robotYaw * Mathf.Deg2Rad);

            // Rotate joystick input by robot's yaw
            float rawForward = Mathf.Clamp(_leftJoystickInput.y, -1f, 1f);
            float rawStrafe = Mathf.Clamp(_leftJoystickInput.x, -1f, 1f);

            forward = rawForward * cos + rawStrafe * sin;
            strafe = -rawForward * sin + rawStrafe * cos;

            UpdateRobotRotate(ref rotate);
        }

        /// <summary>
        /// Updates the robot-centric drive inputs for forward, strafe, and rotation based on joystick input.
        /// </summary>
        /// <param name="forward">A reference to the forward input value, which will be updated to a clamped value between -1 and 1 based on
        /// the vertical axis of the left joystick.</param>
        /// <param name="strafe">A reference to the strafe input value, which will be updated to a clamped value between -1 and 1 based on
        /// the horizontal axis of the left joystick.</param>
        /// <param name="rotate">A reference to the rotation input value, which will be updated by the <see cref="UpdateRobotRotate"/>
        /// method.</param>
        private void UpdateRobotCentricDriveInputs(ref float forward, ref float strafe, ref float rotate)
        {
            forward = Mathf.Clamp(_leftJoystickInput.y, -1f, 1f);
            strafe = Mathf.Clamp(_leftJoystickInput.x, -1f, 1f);

            UpdateRobotRotate(ref rotate);
        }

        /// <summary>
        /// Updates the rotation value for the robot based on the current input from the right joystick.
        /// </summary>
        /// <param name="rotate">A reference to the rotation value to be updated. The value is clamped between -1 and 1 based on the
        /// horizontal input of the right joystick.</param>
        private void UpdateRobotRotate(ref float rotate)
        {
            rotate = Mathf.Clamp(_rightJoystickInput.x, -1f, 1f);
        }

        /// <summary>
        /// Determines whether the given movement inputs are within the defined deadband thresholds.
        /// </summary>
        /// <param name="forward">The forward movement input. Values within the range of -<see cref="_driveDeadband"/> to <see
        /// cref="_driveDeadband"/> are considered within the deadband.</param>
        /// <param name="strafe">The strafe movement input. Values within the range of -<see cref="_driveDeadband"/> to <see
        /// cref="_driveDeadband"/> are considered within the deadband.</param>
        /// <param name="rotate">The rotation input. Values within the range of -<see cref="_rotateDeadband"/> to <see
        /// cref="_rotateDeadband"/> are considered within the deadband.</param>
        /// <returns><see langword="true"/> if all movement inputs are within their respective deadband thresholds; otherwise,
        /// <see langword="false"/>.</returns>
        private bool IsInDeadBand(float forward, float strafe, float rotate)
        {
            return Mathf.Abs(forward) <= _driveDeadband && Mathf.Abs(strafe) <= _driveDeadband && Mathf.Abs(rotate) <= _rotateDeadband;
        }

        /// <summary>
        /// Calculates and returns an array of target vectors based on the specified movement inputs.
        /// </summary>
        /// <remarks>This method combines forward, strafe, and rotational movement inputs to compute the
        /// target vectors for four drive modules. The resulting vectors are stored in the provided <paramref
        /// name="vectors"/> array.</remarks>
        /// <param name="vectors">An array of <see cref="Vector2"/> objects that will be updated with the calculated target vectors. The array
        /// must have a length of at least 4.</param>
        /// <param name="forward">The forward movement input. A positive value moves forward, and a negative value moves backward.</param>
        /// <param name="strafe">The strafe movement input. A positive value moves right, and a negative value moves left.</param>
        /// <param name="rotate">The rotational movement input. A positive value rotates clockwise, and a negative value rotates
        /// counterclockwise.</param>
        /// <returns>The updated array of <see cref="Vector2"/> objects, where each element represents the calculated target
        /// vector for a corresponding drive module.</returns>
        private Vector2[] GetTargetVectors(Vector2[] vectors, float forward, float strafe, float rotate)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 forwardVector = GetDriveForwardVector2() * forward;
                Vector2 strafeVector = GetDriveRightVector2() * strafe;
                Vector2 rotateVector = GetDriveRotateVector2(i) * rotate;

                vectors[i] = forwardVector + strafeVector + rotateVector;
            }

            return vectors;
        }

        /// <summary>
        /// Normalizes an array of 2D vectors so that their magnitudes are scaled relative to the largest magnitude in
        /// the array.
        /// </summary>
        /// <remarks>This method modifies the input array in place. If the largest magnitude in the array
        /// is greater than 1, all vectors are scaled down proportionally to ensure the largest magnitude becomes 1. 
        /// If the array is empty, it is returned unchanged.</remarks>
        /// <param name="vectors">An array of <see cref="Vector2"/> instances to normalize. The array must not be null.</param>
        /// <returns>The input array of vectors, where each vector is scaled by the largest magnitude in the array if it exceeds
        /// 1. If all vectors have magnitudes less than or equal to 1, the array is returned unchanged.</returns>
        private Vector2[] NormalizeVectors(Vector2[] vectors)
        {
            float maxMagnitude = 0f;
            for (int i = 0; i < vectors.Length; i++)
            {
                float mag = vectors[i].magnitude;
                if (mag > maxMagnitude)
                    maxMagnitude = mag;
            }

            if (maxMagnitude > 1f)
            {
                for (int i = 0; i < vectors.Length; i++)
                {
                    vectors[i] /= maxMagnitude;
                }
            }

            return vectors;
        }

        /// <summary>
        /// Updates the steering angle and drive torque for all wheels based on the specified target vectors.
        /// </summary>
        /// <remarks>This method adjusts the steering angle and drive torque of each wheel to align with
        /// the corresponding target vector. If the shortest path to the target angle exceeds 90 degrees, the method
        /// flips the target angle by 180 degrees and inverts the drive torque to ensure proper wheel
        /// alignment.</remarks>
        /// <param name="targetVectors">An array of <see cref="Vector2"/> representing the target directions and magnitudes for each wheel. The
        /// array must contain exactly four elements, one for each wheel.</param>
        private void UpdateAllWheels(Vector2[] targetVectors)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 targetVector = targetVectors[i];
                float targetAngle = Mathf.Atan2(targetVector.x, targetVector.y) * Mathf.Rad2Deg;
                float currentAngle = _wheelTransforms[i].localEulerAngles.y;

                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float steerStep = _steerSpeed * Time.fixedDeltaTime;
                float driveTorque = targetVector.magnitude * _driveSpeed;

                // If the shortest path to the target angle is more than 90 degrees,
                // flip the target angle by 180 and invert the drive torque
                if (Mathf.Abs(angleDiff) > 90f)
                {
                    targetAngle = Mathf.Repeat(targetAngle + 180f, 360f);
                    angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                    driveTorque = -driveTorque;
                }

                // Make the wheel doesn't rotate too fast by limiting the step value
                if (Mathf.Abs(angleDiff) > steerStep)
                {
                    angleDiff = Mathf.Sign(angleDiff) * steerStep;
                }

                // Update the wheel's rotating and wheel collider
                _wheelTransforms[i].localEulerAngles += new Vector3(0f, angleDiff, 0f);
                _wheelColliders[i].steerAngle = _wheelTransforms[i].localEulerAngles.y;
                _wheelColliders[i].motorTorque = driveTorque;
            }
        }
    }
}