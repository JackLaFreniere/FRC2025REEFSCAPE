using UnityEngine;

namespace FRC2025
{
    public class DriveTrainSubsystem : Subsystem
    {
        [Header("Wheel Collider Settings")]
        [SerializeField, Min(1f)] protected float _mass = 20f;
        [SerializeField, Min(0f)] protected float _wheelDampingRate = 0.25f;
        [SerializeField, Min(0f)] protected float _suspensionDistance = 0.3f;
        [SerializeField, Min(0f)] protected float _forceAppPointDistance = 0f;
        [SerializeField] protected Vector3 _center = Vector3.zero;

        [Header("Wheel Collider Suspension Spring Settings")]
        [SerializeField, Min(0f)] protected float _suspensionSpring = 35000f;
        [SerializeField, Min(0f)] protected float _suspensionDamper = 4500f;
        [SerializeField, Min(0f)] protected float _suspensionTargetPosition = 0.5f;

        [Header("Wheel Collider Forward Friction Settings")]
        [SerializeField, Min(0f)] protected float _forwardExtremumSlip = 0.4f;
        [SerializeField, Min(0f)] protected float _forwardExtremumValue = 1f;
        [SerializeField, Min(0f)] protected float _forwardAsymptoteSlip = 0.8f;
        [SerializeField, Min(0f)] protected float _forwardAsymptoteValue = 0.5f;
        [SerializeField, Min(0f)] protected float _forwardStiffness = 1f;

        [Header("Wheel Collider Sideways Friction Settings")]
        [SerializeField, Min(0f)] protected float _sidewaysExtremumSlip = 0.2f;
        [SerializeField, Min(0f)] protected float _sidewaysExtremumValue = 1f;
        [SerializeField, Min(0f)] protected float _sidewaysAsymptoteSlip = 0.5f;
        [SerializeField, Min(0f)] protected float _sidewaysAsymptoteValue = 0.75f;
        [SerializeField, Min(0f)] protected float _sidewaysStiffness = 1f;

        protected WheelCollider[] _wheelColliders;
        protected Vector2 _leftJoystickInput;
        protected Vector2 _rightJoystickInput;

        public override void ApplyTarget(float value)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsAtTarget()
        {
            throw new System.NotImplementedException();
        }

        public void SetDriveInput(Vector2 leftJoyStick) => _leftJoystickInput = leftJoyStick;

        public void SetRotateInput(Vector2 rightJoyStick) => _rightJoystickInput = rightJoyStick;

        protected virtual void Awake() => _wheelColliders = GetComponentsInChildren<WheelCollider>();

#if UNITY_EDITOR
        protected void UpdateWheelColliders()
        {
            foreach (WheelCollider wheel in _wheelColliders)
            {
                if (wheel == null) continue;

                wheel.mass = _mass;
                wheel.wheelDampingRate = _wheelDampingRate;
                wheel.suspensionDistance = _suspensionDistance;
                wheel.forceAppPointDistance = _forceAppPointDistance;
                wheel.center = _center;

                JointSpring suspension = wheel.suspensionSpring;
                suspension.spring = _suspensionSpring;
                suspension.damper = _suspensionDamper;
                suspension.targetPosition = _suspensionTargetPosition;
                wheel.suspensionSpring = suspension;

                WheelFrictionCurve forwardFriction = wheel.forwardFriction;
                forwardFriction.extremumSlip = _forwardExtremumSlip;
                forwardFriction.extremumValue = _forwardExtremumValue;
                forwardFriction.asymptoteSlip = _forwardAsymptoteSlip;
                forwardFriction.asymptoteValue = _forwardAsymptoteValue;
                forwardFriction.stiffness = _forwardStiffness;
                wheel.forwardFriction = forwardFriction;

                WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
                sidewaysFriction.extremumSlip = _sidewaysExtremumSlip;
                sidewaysFriction.extremumValue = _sidewaysExtremumValue;
                sidewaysFriction.asymptoteSlip = _sidewaysAsymptoteSlip;
                sidewaysFriction.asymptoteValue = _sidewaysAsymptoteValue;
                sidewaysFriction.stiffness = _sidewaysStiffness;
                wheel.sidewaysFriction = sidewaysFriction;
            }
        }
#endif
    }
}