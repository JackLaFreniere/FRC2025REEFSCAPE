using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class TankDriveGenerator : DriveTrain
    {
#if UNITY_EDITOR
        [Header("Tank Drive Settings")]
        [SerializeField] private UnitType _wheelUnit = UnitType.Inches;
        [SerializeField, Min(0.01f)] private float _wheelDiameter = 6f;
        [SerializeField, Min(0.01f)] private float _wheelThickness = 11f/8f;
        [SerializeField, Min(0f)] private float _wheelHeightOffset = 1.5f;

        private readonly int _numWheels = 6;
        private float _wheelMultiplier;

        private readonly string _wheelsName = "Wheels";
        private string[] _wheelNames;

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

        private void Start()
        {
            AttemptRemoveSelf(this);
        }

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
            float xOffset = _width * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelThickness * _wheelMultiplier / 2f;
            float yOffset = -_wheelHeightOffset * _wheelMultiplier;
            float zOffset = _length * _robotPerimeterMultiplier / 2f - _driveRailWidth * _driveRailSizeMultiplier - _wheelDiameter * _wheelMultiplier / 2f;
            Quaternion eulerOffset = Quaternion.Euler(0f, 0f, 90f);

            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i] == null) continue;

                float x = (i < 3) ? -xOffset : xOffset;
                float z = (i % 3 == 0) ? zOffset : (i % 3 == 1 ? 0f : -zOffset);
                _wheels[i].transform.SetLocalPositionAndRotation(new Vector3(x, yOffset, z), eulerOffset);
            }
        }

        private void UpdateWheelScale()
        {
            float xOffset = _wheelDiameter * _wheelMultiplier;
            float yOffset = _wheelThickness * _wheelMultiplier / 2f;
            float zOffset = _wheelDiameter * _wheelMultiplier;

            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i] == null) continue;

                _wheels[i].transform.localScale = new Vector3(xOffset, yOffset, zOffset);
            }
        }

        private void UpdateWheelColliders()
        {
            for (int i = 0; i < _wheelColliders.Length; i++)
            {
                if (_wheelColliders[i] == null) continue;

                _wheelColliders[i] = UpdateWheelCollider(_wheelColliders[i], _wheels[i]);
            }
        }

        private WheelCollider UpdateWheelCollider(WheelCollider referenceWheelCollider, GameObject wheel)
        {
            referenceWheelCollider.radius = 0.5f * wheel.transform.localScale.x / wheel.transform.localScale.y;
            referenceWheelCollider.suspensionDistance = 0f;

            return referenceWheelCollider;
        }
#endif
    }
}