using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class TankDrive : DriveTrain
    {
#if UNITY_EDITOR
        [Header("Tank Drive Settings")]
        [SerializeField] private UnitType _wheelUnit = UnitType.Inches;
        [SerializeField, Min(0.01f)] private float _wheelDiameter = 6f;
        [SerializeField, Min(0.01f)] private float _wheelThickness = 11f/8f;
        [SerializeField, Min(0f)] private float _wheelHeightOffset = 1.5f;

        [Header("Drive Settings")]
        [SerializeField] private float _motorForce = 10f;

        private GameObject _wheelsParent;

        private GameObject _leftFrontWheel;
        private GameObject _leftMiddleWheel;
        private GameObject _leftBackWheel;
        private GameObject _rightFrontWheel;
        private GameObject _rightMiddleWheel;
        private GameObject _rightBackWheel;

        private WheelCollider _leftFrontWheelCollider;
        private WheelCollider _leftMiddleWheelCollider;
        private WheelCollider _leftBackWheelCollider;
        private WheelCollider _rightFrontWheelCollider;
        private WheelCollider _rightMiddleWheelCollider;
        private WheelCollider _rightBackWheelCollider;

        private readonly string _wheelsName = "Wheels";
        private readonly string _leftFrontWheelName = "LF_W";
        private readonly string _leftMiddleWheelName = "LM_W";
        private readonly string _leftBackWheelName = "LB_W";
        private readonly string _rightFrontWheelName = "RF_W";
        private readonly string _rightMiddleWheelName = "RM_W";
        private readonly string _rightBackWheelName = "RB_W";

        private float _wheelMultiplier;

        /// <summary>
        /// Initializes the component and sets the default drive train name.
        /// </summary>
        /// <remarks>This method is called automatically by Unity when the script instance is being
        /// loaded. It sets the drive train name to "Tank Drive" as the default value.</remarks>
        private void Awake()
        {
            _driveTrainName = "Tank Drive";
        }

        /// <summary>
        /// Initiates the process of removing the current instance from its context.
        /// </summary>
        /// <remarks>This method triggers the removal of the current instance by invoking the 
        /// <c>AttemptRemoveSelf</c> method. Ensure that the instance is in a valid state  before calling this method to
        /// avoid unexpected behavior.</remarks>
        private void Start()
        {
            AttemptRemoveSelf(this);
        }

        /// <summary>
        /// Updates the state of the object, ensuring all components are initialized and synchronized.
        /// </summary>
        /// <remarks>This method performs a series of updates, including validating and initializing wheel
        /// components,  calculating multipliers, and updating wheel positions, scales, and colliders. It ensures that
        /// the  object is fully initialized and ready for further operations.</remarks>
        protected override void Update()
        {
            _isInitialized = true;
            base.Update();

            ValidateDirectory(ref _wheelsParent, _wheelsName);
            InitializeWheels();

            _wheelMultiplier = UnitToMeters(_wheelUnit);
            UpdateDriveTrainMultipliers();

            UpdateWheelPosition();
            UpdateWheelScale();
            UpdateWheelColliders();
        }

        /// <summary>
        /// Initializes and validates the wheels and their associated colliders for the vehicle.
        /// </summary>
        /// <remarks>This method ensures that all wheels and their corresponding colliders are properly
        /// set up  and associated with the vehicle. It validates each wheel and assigns the appropriate references  to
        /// ensure correct functionality.</remarks>
        private void InitializeWheels()
        {
            _leftFrontWheel = ValidateWheel(_leftFrontWheel, ref _leftFrontWheelCollider, _leftFrontWheelName);
            _leftMiddleWheel = ValidateWheel(_leftMiddleWheel, ref _leftMiddleWheelCollider, _leftMiddleWheelName);
            _leftBackWheel = ValidateWheel(_leftBackWheel, ref _leftBackWheelCollider, _leftBackWheelName);
            _rightFrontWheel = ValidateWheel(_rightFrontWheel, ref _rightFrontWheelCollider, _rightFrontWheelName);
            _rightMiddleWheel = ValidateWheel(_rightMiddleWheel, ref _rightMiddleWheelCollider, _rightMiddleWheelName);
            _rightBackWheel = ValidateWheel(_rightBackWheel, ref _rightBackWheelCollider, _rightBackWheelName);
        }

        /// <summary>
        /// Validates and retrieves a wheel GameObject by name, creating a new one if it does not exist.
        /// </summary>
        /// <remarks>If the specified wheel GameObject does not exist, a new wheel GameObject is created
        /// as a primitive cylinder,  assigned the specified name, and parented to the wheels container. A <see
        /// cref="WheelCollider"/> is also added  to the newly created GameObject.</remarks>
        /// <param name="referenceWheel">The existing wheel GameObject to validate. If null, a new wheel will be created.</param>
        /// <param name="wheel">A reference to the <see cref="WheelCollider"/> associated with the wheel. This will be updated to the  <see
        /// cref="WheelCollider"/> of the validated or newly created wheel.</param>
        /// <param name="name">The name of the wheel to find or assign to the newly created wheel.</param>
        /// <returns>The validated wheel GameObject if it exists, or a newly created wheel GameObject if it does not.</returns>
        private GameObject ValidateWheel(GameObject referenceWheel, ref WheelCollider wheel, string name)
        {
            if (referenceWheel != null) return referenceWheel;

            Transform wheelTransform = _wheelsParent.transform.Find(name);
            if (wheelTransform != null)
            {
                wheel = wheelTransform.GetComponent<WheelCollider>();
                return wheelTransform.gameObject;
            }
            else
            {
                referenceWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                referenceWheel.name = name;
                referenceWheel.transform.SetParent(_wheelsParent.transform);
                wheel = referenceWheel.AddComponent<WheelCollider>();

                return referenceWheel;
            }
        }

        /// <summary>
        /// Updates the positions and rotations of the robot's wheels based on the current configuration parameters.
        /// </summary>
        /// <remarks>This method calculates the offsets for the wheel positions and applies them to the
        /// left and right wheels of the robot. The positions are adjusted relative to the robot's dimensions, drive
        /// rail size, and wheel dimensions. The wheels are rotated to align with the robot's expected
        /// orientation.</remarks>
        private void UpdateWheelPosition()
        {
            float xOffset = _width * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelThickness * _wheelMultiplier / 2f;
            float yOffset = -_wheelHeightOffset * _wheelMultiplier;
            float zOffset = _length * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelDiameter * _wheelMultiplier / 2f;
            Quaternion eulerOffset = Quaternion.Euler(0f, 0f, 90f);

            _leftFrontWheel.transform.SetLocalPositionAndRotation(new Vector3(-xOffset, yOffset, zOffset), eulerOffset);
            _leftMiddleWheel.transform.SetLocalPositionAndRotation(new Vector3(-xOffset, yOffset, 0f), eulerOffset);
            _leftBackWheel.transform.SetLocalPositionAndRotation(new Vector3(-xOffset, yOffset, -zOffset), eulerOffset);

            _rightFrontWheel.transform.SetLocalPositionAndRotation(new Vector3(xOffset, yOffset, zOffset), eulerOffset);
            _rightMiddleWheel.transform.SetLocalPositionAndRotation(new Vector3(xOffset, yOffset, 0f), eulerOffset);
            _rightBackWheel.transform.SetLocalPositionAndRotation(new Vector3(xOffset, yOffset, -zOffset), eulerOffset);
        }

        /// <summary>
        /// Updates the scale of all wheels based on the current wheel diameter, thickness, and multiplier values.
        /// </summary>
        /// <remarks>This method adjusts the local scale of each wheel to ensure they are sized
        /// proportionally according to the specified wheel dimensions. The scaling factors are derived from the wheel
        /// diameter, thickness, and a multiplier, and are applied uniformly to all wheels.</remarks>
        private void UpdateWheelScale()
        {
            float xOffset = _wheelDiameter * _wheelMultiplier;
            float yOffset = _wheelThickness * _wheelMultiplier / 2f;
            float zOffset = _wheelDiameter * _wheelMultiplier;

            _leftFrontWheel.transform.localScale = new Vector3(xOffset, yOffset, zOffset);
            _leftMiddleWheel.transform.localScale = new Vector3(xOffset, yOffset, zOffset);
            _leftBackWheel.transform.localScale = new Vector3(xOffset, yOffset, zOffset);

            _rightFrontWheel.transform.localScale = new Vector3(xOffset, yOffset, zOffset);
            _rightMiddleWheel.transform.localScale = new Vector3(xOffset, yOffset, zOffset);
            _rightBackWheel.transform.localScale = new Vector3(xOffset, yOffset, zOffset);
        }

        /// <summary>
        /// Updates the wheel colliders for all wheels of the vehicle.
        /// </summary>
        /// <remarks>This method ensures that the wheel colliders are synchronized with their
        /// corresponding wheel transforms. It updates the colliders for the left and right front, middle, and back
        /// wheels.</remarks>
        private void UpdateWheelColliders()
        {
            _leftFrontWheelCollider = UpdateWheelCollider(_leftFrontWheelCollider, _leftFrontWheel);
            _leftMiddleWheelCollider = UpdateWheelCollider(_leftMiddleWheelCollider, _leftMiddleWheel);
            _leftBackWheelCollider = UpdateWheelCollider(_leftBackWheelCollider, _leftBackWheel);

            _rightFrontWheelCollider = UpdateWheelCollider(_rightFrontWheelCollider, _rightFrontWheel);
            _rightMiddleWheelCollider = UpdateWheelCollider(_rightMiddleWheelCollider, _rightMiddleWheel);
            _rightBackWheelCollider = UpdateWheelCollider(_rightBackWheelCollider, _rightBackWheel);
        }

        /// <summary>
        /// Updates the properties of a <see cref="WheelCollider"/> based on the specified wheel's transform.
        /// </summary>
        /// <remarks>The radius of the <paramref name="referenceWheelCollider"/> is calculated as half the
        /// ratio of the wheel's local X scale to its local Y scale. The suspension distance is set to zero.</remarks>
        /// <param name="referenceWheelCollider">The <see cref="WheelCollider"/> to update.</param>
        /// <param name="wheel">The <see cref="GameObject"/> representing the wheel, whose transform is used to calculate the collider's
        /// properties.</param>
        /// <returns>The updated <see cref="WheelCollider"/> with modified radius and suspension distance.</returns>
        private WheelCollider UpdateWheelCollider(WheelCollider referenceWheelCollider, GameObject wheel)
        {
            referenceWheelCollider.radius = 0.5f * wheel.transform.localScale.x / wheel.transform.localScale.y;
            referenceWheelCollider.suspensionDistance = 0f;

            return referenceWheelCollider;
        }
#endif
    }
}