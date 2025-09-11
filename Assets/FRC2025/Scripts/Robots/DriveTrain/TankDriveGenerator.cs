using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class TankDriveGenerator : DriveTrainGenerator<TankDriveSubsystem>
    {
#if UNITY_EDITOR
        [Header("Tank Drive Settings")]
        [SerializeField] private UnitType _wheelUnit = UnitType.Inches;
        [SerializeField, Min(0.01f)] private float _wheelDiameter = 6f;
        [SerializeField, Min(0.01f)] private float _wheelThickness = 11f/8f;
        [SerializeField, Min(0f)] private float _wheelHeightOffset = 1.5f;

        [Header("Drive Settings")]
        [SerializeField] protected float _driveForce = 10f;

        private readonly int _numWheels = 6;
        private float _wheelMultiplier;

        private readonly string _wheelsParentName = "Wheels";
        private string[] _wheelNames;

        /// <summary>
        /// Initializes the drive train name, wheel names, and allocates arrays for wheels and wheel colliders.
        /// </summary>
        /// <remarks>This method sets up the initial configuration for the drive train and prepares the
        /// necessary data structures for managing the wheels and their colliders. It is typically called during the
        /// Unity lifecycle to ensure the object is properly initialized before use.</remarks>
        private void Awake()
        {
            _driveTrainName = "Tank Drive";

            _wheelNames = new string[]
            {
                "LF_W", "LM_W", "LB_W",
                "RF_W", "RM_W", "RB_W"
            };

            _wheels = new GameObject[_numWheels];
            _wheelColliders = new WheelCollider[_numWheels];
        }

        /// <summary>
        /// Initiates the process of removing the current instance from its context.
        /// </summary>
        /// <remarks>This method triggers the removal of the current instance by invoking the 
        /// <c>AttemptRemoveSelf</c> method. Ensure that the instance is in a valid state before calling this method to
        /// avoid unexpected behavior.</remarks>
        private void Start()
        {
            AttemptRemoveSelf(this);
        }

        /// <summary>
        /// Updates the state of the object, ensuring all components are initialized and synchronized.
        /// </summary>
        /// <remarks>This method performs several operations to prepare and update the object's wheels and
        /// drivetrain: it validates the directory structure, initializes wheel components, calculates necessary
        /// multipliers, and updates the positions, scales, and colliders of the wheels. This method should be called as
        /// part of the object's update lifecycle to ensure proper functionality.</remarks>
        protected override void Update()
        {
            _isInitialized = true;
            base.Update();

            ValidateDirectory(ref _wheelsParent, _wheelsParentName);
            InitializeWheels();

            _wheelMultiplier = UnitToMeters(_wheelUnit);
            UpdateDriveTrainMultipliers();

            UpdateWheelPosition();
            UpdateWheelScale();
            UpdateWheelColliders();
        }

        /// <summary>
        /// Initializes the wheels of the vehicle by validating and configuring each wheel.
        /// </summary>
        /// <remarks>This method iterates through all wheels in the vehicle, ensuring that each wheel is
        /// properly validated and associated with its corresponding wheel collider and name. It updates the internal
        /// state of the wheels to ensure they are ready for use.</remarks>
        private void InitializeWheels()
        {
            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i] = ValidateWheel(_wheels[i], ref _wheelColliders[i], _wheelNames[i]);
            }
        }

        /// <summary>
        /// Validates and retrieves a wheel GameObject by name, creating a new one if it does not exist.
        /// </summary>
        /// <remarks>If the <paramref name="referenceWheel"/> is <see langword="null"/>, the method
        /// attempts to find a child GameObject with the specified <paramref name="name"/> under the parent object. If
        /// found, the associated <see cref="WheelCollider"/> is retrieved. If no such GameObject exists, a new wheel
        /// GameObject is created with a <see cref="WheelCollider"/> component and added as a child of the parent
        /// object.</remarks>
        /// <param name="referenceWheel">The existing wheel GameObject to validate. If this is not <see langword="null"/>, it will be returned as-is.</param>
        /// <param name="wheel">A reference to the <see cref="WheelCollider"/> associated with the wheel. This will be updated to the <see
        /// cref="WheelCollider"/> of the retrieved or newly created wheel.</param>
        /// <param name="name">The name of the wheel to search for or assign to the newly created wheel.</param>
        /// <returns>The validated or newly created wheel GameObject. If a wheel with the specified name exists, it is returned;
        /// otherwise, a new wheel GameObject is created, configured, and returned.</returns>
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
        /// Updates the positions and rotations of the robot's wheels based on the current configuration.
        /// </summary>
        /// <remarks>This method calculates the offsets for each wheel using the robot's dimensions, wheel
        /// properties, and configuration multipliers. It then applies the calculated positions and a fixed rotation 
        /// to each wheel in the <see cref="_wheels"/> array.</remarks>
        private void UpdateWheelPosition()
        {
            float xOffset = _width * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelThickness * _wheelMultiplier / 2f;
            float yOffset = -_wheelHeightOffset * _wheelMultiplier;
            float zOffset = _length * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelDiameter * _wheelMultiplier / 2f;
            Quaternion eulerOffset = Quaternion.Euler(0f, 0f, 90f);

            Vector3[] wheelPositionOffsets =
            {
                new (-xOffset, yOffset,  zOffset), // Left Front
                new (-xOffset, yOffset,  0f),      // Left Middle
                new (-xOffset, yOffset, -zOffset), // Left Back
                new ( xOffset, yOffset,  zOffset), // Right Front
                new ( xOffset, yOffset,  0f),      // Right Middle
                new ( xOffset, yOffset, -zOffset), // Right Back
            };

            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i].transform.SetLocalPositionAndRotation(wheelPositionOffsets[i], eulerOffset);
            }
        }

        /// <summary>
        /// Updates the scale of all wheels in the collection based on the current wheel diameter, wheel thickness, and
        /// a scaling multiplier.
        /// </summary>
        /// <remarks>This method adjusts the local scale of each wheel in the <c>_wheels</c> array. The
        /// scale is calculated using the wheel diameter, wheel thickness, and a multiplier to ensure consistent
        /// scaling across all dimensions.</remarks>
        private void UpdateWheelScale()
        {
            float xOffset = _wheelDiameter * _wheelMultiplier;
            float yOffset = _wheelThickness * _wheelMultiplier / 2f;
            float zOffset = _wheelDiameter * _wheelMultiplier;

            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i].transform.localScale = new Vector3(xOffset, yOffset, zOffset);
            }
        }

        /// <summary>
        /// Updates the state of all wheel colliders to match the corresponding wheels.
        /// </summary>
        /// <remarks>This method iterates through all wheel colliders and updates each one based on the
        /// state of its associated wheel. Ensure that the arrays of wheel colliders and wheels are properly initialized
        /// and have the same length before calling this method.</remarks>
        private void UpdateWheelColliders()
        {
            for (int i = 0; i < _wheelColliders.Length; i++)
            {
                _wheelColliders[i] = UpdateWheelCollider(_wheelColliders[i], _wheels[i]);
            }
        }

        /// <summary>
        /// Updates the properties of a <see cref="WheelCollider"/> based on the scale of the specified wheel object.
        /// </summary>
        /// <param name="referenceWheelCollider">The <see cref="WheelCollider"/> to be updated.</param>
        /// <param name="wheel">The <see cref="GameObject"/> representing the wheel, whose scale is used to calculate the collider's
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