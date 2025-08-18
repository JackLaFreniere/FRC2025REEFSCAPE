    using UnityEngine;

    public class Shoulder : MonoBehaviour
    {
        [SerializeField] private HingeJoint hinge;

        private readonly float tolerance = SubsystemHelper.defaultAngularTolerance;

        private const float shoulderOffset = 90f;
        private float targetRotation;

        /// <summary>
        /// Sets the target rotation for the Shoulder subsystem.
        /// </summary>
        /// <param name="targetAngle">The desired angle for the shoulder.</param>
        public void SetTargetRotation(float targetAngle)
        {
            targetRotation = -targetAngle + shoulderOffset;

            SubsystemHelper.UpdateHingeJointSpring(hinge, targetRotation);
    }

        /// <summary>
        /// Determines if the Shoulder is at its target rotation within a specified tolerance.
        /// </summary>
        /// <returns>If within tolerance</returns>
        public bool IsAtTargetRotation()
        {
            return SubsystemHelper.IsAtRotation(transform.localRotation, targetRotation, tolerance);
        }
    }