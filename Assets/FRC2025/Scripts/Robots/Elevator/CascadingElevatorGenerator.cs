using System.Collections.Generic;
using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class CascadingElevatorGenerator : Generator<CascadingElevatorSubsystem>
    {
#if UNITY_EDITOR

        #region Serialized Fields

        [Header("Tubing Dimensions")]
        [SerializeField] private UnitType _tubingUnit = UnitType.Inches;
        [SerializeField, Min(0f)] private float _tubingLength = 2f;
        [SerializeField, Min(0f)] private float _tubingWidth = 1f;
        [SerializeField, Min(0f)] private float _tubingHeight = 12f;
        [SerializeField, Min(0f)] private float _tubingSpacing = 0.125f;

        [Header("Elevator Positioning")]
        [SerializeField] private UnitType _offsetUnit = UnitType.Meters;
        [SerializeField] private float _stageXOffset = 0f;
        [SerializeField] private float _stageYOffset = 0f;
        [SerializeField] private float _stageZOffset = 0f;
        [SerializeField] private float _stageXRotation = 0f;
        [SerializeField] private float _stageYRotation = 0f;
        [SerializeField] private float _stageZRotation = 0f;

        [Header("Stage Configuration")]
        [SerializeField] private UnitType _baseStageUnit = UnitType.Meters;
        [SerializeField, Min(0f)] private float _baseStageWidth = 1f;
        [SerializeField, Min(2f)] private int _numStages = 2;

        #endregion

        #region Private Fields

        private float _tubingUnitMultiplier;
        private float _offsetUnitMultiplier;
        private float _baseStageUnitMultiplier;

        private GameObject _baseStageParent;
        private GameObject _topStageParent;
        private GameObject _intermediateStagesParent;
        private readonly List<GameObject> _intermediateStagesSubParents = new();

        private GameObject[] _baseStage = new GameObject[2];
        private GameObject[] _topStage = new GameObject[2];
        private readonly List<GameObject[]> _intermediateStages = new();

        private GameObject[] _runtimeStages;
        private bool _hasInitializedSubsystem;

        private readonly string _baseStageParentName = "Base Stage";
        private readonly string _topStageParentName = "Top Stage";
        private readonly string _intermediateStagesParentName = "Intermediate Stages";
        private readonly string _intermediateStageName = "Intermediate Stage";
        
        private readonly int _tubesPerStage = 2;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _name = "Cascading Elevator";
            _hasInitializedSubsystem = false;
        }

        protected override void Update()
        {
            base.Update();

            UpdateUnitMultipliers();

            if (Application.isPlaying)
            {
                HandleRunTimeUpdate();
                return;
            }

            HandleEditorUpdate();
        }

        private void HandleRunTimeUpdate()
        {
            if (!_hasInitializedSubsystem)
            {
                InitializeSubsystem();
            }

            foreach (GameObject stage in _runtimeStages)
            {
                ConstrainMinPosition(stage);
            }
        }

        private void InitializeSubsystem()
        {
            _runtimeStages = GetStageGameObjects();
            GetComponent<CascadingElevatorSubsystem>().SetElevatorStages(_runtimeStages);
            _hasInitializedSubsystem = true;
        }

        private GameObject[] GetStageGameObjects()
        {
            GameObject[] stages = new GameObject[_numStages];
            stages[0] = GameObject.Find(_baseStageParentName);

            for (int i = 1; i <= _numStages - 2; i++)
            {
                stages[i] = GameObject.Find($"{_intermediateStageName} {i}");
            }

            stages[^1] = GameObject.Find(_topStageParentName);

            return stages;
        }

        private void ConstrainMinPosition(GameObject stage)
        {
            if (stage == null) return;
            
            if (stage.transform.localPosition.y < 0f)
            {
                Vector3 pos = stage.transform.localPosition;
                pos.y = 0f;
                stage.transform.localPosition = pos;
            }
        }

        #endregion

        #region Editor Methods

        private void HandleEditorUpdate()
        {
            ValidateStageHierarchy();
            ConfigureAllStages();
            SetupPhysicsJoints();
            UpdateElevatorTransform();
        }
        
        private void ValidateStageHierarchy()
        {
            ValidateDirectory(ref _baseStageParent, _baseStageParentName);
            ValidateDirectory(ref _topStageParent, _topStageParentName);
            ValidateDirectory(ref _intermediateStagesParent, _intermediateStagesParentName);

            _baseStage = ValidateStageTubes(_baseStage, _baseStageParent, _baseStageParentName);
            _topStage = ValidateStageTubes(_topStage, _topStageParent, _topStageParentName);

            ValidateIntermediateStages();
        }

        private void ValidateIntermediateStages()
        {
            int intermediateCount = Mathf.Max(0, _numStages - 2);

            // Adds subparents when needed
            while (_intermediateStagesSubParents.Count < intermediateCount)
            {
                int stageIndex = _intermediateStagesSubParents.Count + 1;
                string name = $"{_intermediateStageName} {stageIndex}";
                
                GameObject subParent = null;
                ValidateDirectory(ref subParent, name, _intermediateStagesParent);
                _intermediateStagesSubParents.Add(subParent);
            }

            // Removes excess subparents when needed
            while (_intermediateStagesSubParents.Count > intermediateCount)
            {
                GameObject toRemove = _intermediateStagesSubParents[^1];
                if (toRemove != null)
                    DestroyImmediate(toRemove);
                _intermediateStagesSubParents.RemoveAt(_intermediateStagesSubParents.Count - 1);
            }

            // Ensure intermediate stages list matches count
            while (_intermediateStages.Count < intermediateCount)
            {
                _intermediateStages.Add(new GameObject[_tubesPerStage]);
            }

            while (_intermediateStages.Count > intermediateCount)
            {
                DestroyStageTubes(_intermediateStages[^1]);
                _intermediateStages.RemoveAt(_intermediateStages.Count - 1);
            }

            // Validate each intermediate stage's cubes under its subparent
            for (int i = 0; i < intermediateCount; i++)
            {
                GameObject subParent = _intermediateStagesSubParents[i];
                string stageName = $"{_intermediateStageName} {i + 1}";
                _intermediateStages[i] = ValidateStageTubes(_intermediateStages[i], subParent, stageName);
            }
        }

        private GameObject[] ValidateStageTubes(GameObject[] tubes, GameObject parent, string name)
        {
            if (tubes == null || tubes.Length != _tubesPerStage)
                tubes = new GameObject[_tubesPerStage];

            string[] suffixes = { " Left", " Right" };
            for (int i = 0; i < _tubesPerStage; i++)
            {
                string tubeName = $"{name} {suffixes[i]}";
                if (tubes[i] == null)
                {
                    Transform t = parent.transform.Find(tubeName);
                    tubes[i] = t != null ? t.gameObject : GameObject.CreatePrimitive(PrimitiveType.Cube);
                }

                tubes[i].name = tubeName;
                tubes[i].transform.SetParent(parent.transform, false);
            }

            return tubes;
        }
        
        private void DestroyStageTubes(GameObject[] tubes)
        {
            if (tubes == null) return;

            foreach (GameObject tube in tubes)
            {
                if (tube != null)
                {
                    DestroyImmediate(tube);
                }
            }
        }

        #endregion

        #region Configuration Methods

        private void ConfigureAllStages()
        {
            ConfigureStage(1, _baseStage);
            ConfigureStage(_numStages, _topStage);

            for (int i = 2; i < _numStages; i++)
            {
                ConfigureStage(i, _intermediateStages[i - 2]);
            }
        }

        private void ConfigureStage(int stageNumber, GameObject[] stageObjects)
        {
            if (stageObjects == null || stageObjects.Length != _tubesPerStage) return;

            int inwardIndex = stageNumber - 1;
            float width = _tubingWidth * _tubingUnitMultiplier;
            float height = _tubingHeight * _tubingUnitMultiplier;
            float length = _tubingLength * _tubingUnitMultiplier;
            float spacing = _tubingSpacing * _tubingUnitMultiplier;

            float zOffset = inwardIndex * (width + spacing);

            float baseStageWidth = _baseStageWidth * _baseStageUnitMultiplier;
            float leftX = -baseStageWidth / 2f + width / 2f + zOffset;
            float rightX = baseStageWidth / 2f - width / 2f - zOffset;

            Vector3 leftPos = new(leftX, height / 2f, 0f);
            Vector3 rightPos = new(rightX, height / 2f, 0f);
            Vector3 scale = new(width, height, length);

            ConfigureTube(stageObjects[0], scale, leftPos);
            ConfigureTube(stageObjects[1], scale, rightPos);
        }

        private void ConfigureTube(GameObject tube, Vector3 scale, Vector3 position)
        {
            if (tube == null) return;

            tube.transform.localScale = scale;
            tube.transform.localPosition = position;
        }

        private void UpdateElevatorTransform()
        {
            Vector3 positionOffset = new(
                _stageXOffset * _offsetUnitMultiplier,
                _stageYOffset * _offsetUnitMultiplier,
                _stageZOffset * _offsetUnitMultiplier);

            Quaternion rotationOffset = Quaternion.Euler(
                _stageXRotation,
                _stageYRotation,
                _stageZRotation);

            transform.SetLocalPositionAndRotation(positionOffset, rotationOffset);
        }

        private void UpdateUnitMultipliers()
        {
            _tubingUnitMultiplier = RobotHelper.UnitToMeters(_tubingUnit);
            _offsetUnitMultiplier = RobotHelper.UnitToMeters(_offsetUnit);
            _baseStageUnitMultiplier = RobotHelper.UnitToMeters(_baseStageUnit);
        }

        #endregion

        #region Physics Joint Setup

        private void SetupPhysicsJoints()
        {
            SetupBaseStage();
            SetupStageJointChain();
        }

        private void SetupBaseStage()
        {
            if (_baseStageParent == null) return;

            DestroyImmediate(_baseStageParent.GetComponent<ConfigurableJoint>());

            InitializedRigidBody(_baseStageParent, true);
        }

        private void SetupStageJointChain()
        {
            if (_baseStageParent == null) return;

            Rigidbody baseRb = _baseStageParent.GetComponent<Rigidbody>();

            List<GameObject> movableStages = new();
            if (_intermediateStagesSubParents != null && _intermediateStagesSubParents.Count > 0)
                movableStages.AddRange(_intermediateStagesSubParents);

            if (_topStageParent != null)
                movableStages.Add(_topStageParent);

            Rigidbody previousRb = baseRb;
            foreach (GameObject stage in movableStages)
            {
                if (stage == null) continue;

                InitializeStageJoint(stage);

                ConfigurableJoint joint = stage.GetComponent<ConfigurableJoint>();
                Rigidbody rb = stage.GetComponent<Rigidbody>();

                if (joint != null && rb != null)
                {
                    joint.connectedBody = previousRb;
                    previousRb = rb;
                }
            }
        }

        private void InitializedRigidBody(GameObject gameObject, bool kinematic = false)
        {
            if (gameObject == null) return;
            Rigidbody rb = gameObject.GetOrAddComponent<Rigidbody>();
            rb.isKinematic = kinematic;
            rb.useGravity = false;
        }

        private void InitializeStageJoint(GameObject stage)
        {
            if (stage == null) return;

            InitializedRigidBody(stage);

            ConfigurableJoint joint = stage.GetOrAddComponent<ConfigurableJoint>();

            ConfigureVerticalSliderJoint(joint);
        }

        private void ConfigureVerticalSliderJoint(ConfigurableJoint joint)
        {
            joint.anchor = Vector3.zero;
            joint.axis = Vector3.forward;
            joint.secondaryAxis = Vector3.zero;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.linearLimit = new SoftJointLimit
            {
                limit = _tubingHeight * _tubingUnitMultiplier
            };

            joint.yDrive = new JointDrive
            {
                positionSpring = 1000f,
                positionDamper = 100f,
                maximumForce = Mathf.Infinity
            };
        }

        #endregion

#endif
    }
}