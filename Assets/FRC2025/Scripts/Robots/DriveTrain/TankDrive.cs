using Unity.VisualScripting;
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
        [SerializeField] private float motorForce = 1500f;

        private WheelCollider[] leftWheels;
        private WheelCollider[] rightWheels;

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
        /// Initiates the process of attempting to remove the current instance from its context.
        /// </summary>
        /// <remarks>This method triggers the removal operation by calling the appropriate internal logic.
        /// Ensure that the instance is in a valid state before invoking this method.</remarks>
        private void Start()
        {
            AttemptRemoveSelf(this);
        }

        /// <summary>
        /// Updates the state of the object, including wheel configuration and drive train multipliers.
        /// </summary>
        /// <remarks>This method validates and initializes the wheel configuration, updates scaling
        /// factors,  and adjusts the positions and scales of the wheels. It is called as part of the update cycle  and
        /// ensures that the object remains in a consistent state.</remarks>
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
            //UpdateWheelColliders();
        }

        /// <summary>
        /// Initializes and validates the wheels of the vehicle, ensuring that each wheel and its corresponding collider
        /// are properly configured.
        /// </summary>
        /// <remarks>This method validates the state of each wheel and its associated collider by invoking
        /// the  <c>ValidateWheel</c> method. It ensures that all wheels are correctly initialized and ready for use.
        /// This method is intended to be called during the setup or initialization phase of the vehicle.</remarks>
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
        /// <remarks>If the <paramref name="referenceWheel"/> is <see langword="null"/>, the method
        /// attempts to find a child GameObject with the specified <paramref name="name"/> under the parent object. If
        /// found, the associated <see cref="WheelCollider"/> is retrieved and assigned to <paramref name="wheel"/>. If
        /// no such GameObject exists, a new wheel GameObject is created, configured with a <see cref="WheelCollider"/>,
        /// and returned.</remarks>
        /// <param name="referenceWheel">The existing wheel GameObject to validate. If this is not <see langword="null"/>, it will be returned as-is.</param>
        /// <param name="wheel">A reference to the <see cref="WheelCollider"/> associated with the wheel. This will be updated to the <see
        /// cref="WheelCollider"/> of the validated or newly created wheel.</param>
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
                referenceWheel.AddComponent<WheelCollider>();

                return referenceWheel;
            }
        }

        /// <summary>
        /// Updates the positions and rotations of the robot's wheels based on the current configuration parameters.
        /// </summary>
        /// <remarks>This method calculates the offsets for the wheel positions and applies them to the
        /// left and right wheels  of the robot. The positions are adjusted based on the robot's dimensions, wheel
        /// dimensions, and other  configuration multipliers. The wheels are rotated to align with the robot's expected
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
        /// <remarks>This method adjusts the local scale of the left and right wheels to ensure they are
        /// sized proportionally according to the specified wheel dimensions. The scaling factors are derived from the
        /// wheel diameter, thickness, and a multiplier, and are applied uniformly to all wheels.</remarks>
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

        //private void UpdateWheelColliders()
        //{
        //    _leftFrontWheelCollider = UpdateWheelCollider(_leftFrontWheelCollider, _leftFrontWheelName);
        //}

        //private WheelCollider UpdateWheelCollider(WheelCollider wheelCollider)
        //{
        //    if (wheelCollider == null) return null;

        //    GameObject wheelGameObject = wheelCollider.gameObject;
        //    WheelCollider wheel = wheelGameObject.GetComponent<WheelCollider>();
        //    if (wheel != null) return wheel;

        //    wheelCollider.radius = 2f / 3f / transform.localScale.x;
        //    wheelCollider.suspensionDistance = 0.1f;

        //    return wheelCollider;
        //}
#endif
    }
}