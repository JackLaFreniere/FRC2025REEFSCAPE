using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class SwerveDriveGenerator : DriveTrainGenerator<SwerveDriveSubsystem>
    {
#if UNITY_EDITOR
        [Header("Swerve Drive Settings")]
        [SerializeField] private UnitType _wheelUnit = UnitType.Inches;
        [SerializeField, Min(0.01f)] private float _wheelDiameter = 5f;
        [SerializeField, Min(0.01f)] private float _wheelThickness = 2f;
        [SerializeField, Min(0f)] private float _wheelHeightOffset = 1.5f;
        [SerializeField, Range(-1f, 1f)] private float _frontBackOffset = 0.8f; // -1 = back, 1 = front
        [SerializeField, Range(-1f, 1f)] private float _leftRightOffset = 0.8f; // -1 = left, 1 = right

        private readonly int _numWheels = 4;
        private float _wheelMultiplier;

        private readonly string _wheelsParentName = "Wheels";
        private readonly string[] _wheelNames = new string[]
        {
            "FL_W", // Front Left
            "FR_W", // Front Right
            "BL_W", // Back Left
            "BR_W"  // Back Right
        };

        private void Awake()
        {
            _driveTrainName = "Swerve Drive";
            _wheels = new GameObject[_numWheels];
            _wheelColliders = new WheelCollider[_numWheels];
        }

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

        private void InitializeWheels()
        {
            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i] = ValidateWheel(_wheels[i], ref _wheelColliders[i], _wheelNames[i]);
            }
        }

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

        private void UpdateWheelPosition()
        {
            float xEdge = _width * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelThickness * _wheelMultiplier / 2f;
            float zEdge = _length * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelDiameter * _wheelMultiplier / 2f;
            float yOffset = -_wheelHeightOffset * _wheelMultiplier;
            Quaternion eulerOffset = Quaternion.Euler(0f, 0f, 90f);

            // Adjustable offsets for each wheel
            Vector3[] wheelPositionOffsets =
            {
                new (-xEdge * _leftRightOffset, yOffset,  zEdge * _frontBackOffset), // Front Left
                new ( xEdge * _leftRightOffset, yOffset,  zEdge * _frontBackOffset), // Front Right
                new (-xEdge * _leftRightOffset, yOffset, -zEdge * _frontBackOffset), // Back Left
                new ( xEdge * _leftRightOffset, yOffset, -zEdge * _frontBackOffset), // Back Right
            };

            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i].transform.localPosition = wheelPositionOffsets[i];
                //_wheels[i].transform.SetLocalPositionAndRotation(wheelPositionOffsets[i], eulerOffset);
            }
        }

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

        private void UpdateWheelColliders()
        {
            for (int i = 0; i < _wheelColliders.Length; i++)
            {
                _wheelColliders[i] = UpdateWheelCollider(_wheelColliders[i], _wheels[i]);
            }
        }

        private WheelCollider UpdateWheelCollider(WheelCollider referenceWheelCollider, GameObject wheel)
        {
            referenceWheelCollider.radius = 0.5f * wheel.transform.localScale.x / wheel.transform.localScale.y;
            referenceWheelCollider.suspensionDistance = 0f;
            referenceWheelCollider.mass = 1f;
            referenceWheelCollider.brakeTorque = 0f;
            return referenceWheelCollider;
        }
#endif
    }
}