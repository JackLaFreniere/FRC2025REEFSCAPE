using System.Collections.Generic;
using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class CascadingElevatorGenerator : Generator<CascadingElevatorSubsystem>
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Header("Cascading Elevator Settings")]
        [SerializeField] private UnitType _tubingUnit = UnitType.Inches;
        [SerializeField, Min(0)] private float _tubingLength = 2f;
        [SerializeField, Min(0f)] private float _tubingWidth = 1f;
        [SerializeField, Min(0f)] private float _tubingHeight = 12f;
        [SerializeField, Min(0f)] private float _tubingSpacing = 0.125f;

        [Header("Stage Settings")]
        [SerializeField] private UnitType _offsetUnit = UnitType.Meters;
        [SerializeField] private float _stageXOffset = 0f;
        [SerializeField] private float _stageYOffset = 0f;
        [SerializeField] private float _stageZOffset = 0f;
        [SerializeField] private float _stageRotation = 0f;
        [Space(10)]
        [SerializeField] private UnitType _baseStageUnit = UnitType.Meters;
        [SerializeField, Min(0f)] private float _baseStageWidth = 1f;
        [SerializeField, Min(2)] private int _numStages = 2;

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

        private readonly string _baseStageParentName = "Base Stage";
        private readonly string _topStageParentName = "Top Stage";
        private readonly string _intermediateStagesParentName = "Intermediate Stages";
        private readonly string _intermediateStageName = "Intermediate Stage";

        private bool _hasUpdatedSubstem;
#pragma warning restore CS0414

        private void Awake()
        {
            _name = "Cascading Elevator";
            _hasUpdatedSubstem = false;
        }

        protected override void Update()
        {
            base.Update();

            if (Application.isPlaying)
            {
                if (!_hasUpdatedSubstem)
                {
                    SetSubsystemStages();
                    _hasUpdatedSubstem = true;
                }

                return;
            }

            UpdateElevatorMultipliers();

            // Validate main parents
            ValidateDirectory(ref _baseStageParent, _baseStageParentName);
            ValidateDirectory(ref _topStageParent, _topStageParentName);
            ValidateDirectory(ref _intermediateStagesParent, _intermediateStagesParentName);

            InitializeConfigurableJoint(_topStageParent);

            // Validate base and top stages (each has 2 cubes)
            _baseStage = ValidateStage(_baseStage, _baseStageParent, _baseStageParentName);
            _topStage = ValidateStage(_topStage, _topStageParent, _topStageParentName);

            // Handle interme Stages();
            ValidateIntermediateStages();

            // Ensure the joint chain connects correctly: intermediates -> top, lowest intermediate connects to base stage.
            SetupStageJointChain();

            UpdateElevatorOffset();

            ConfigureStage(1, _baseStage);
            ConfigureStage(_numStages, _topStage);

            for (int i = 2; i < _numStages; i++)
            {
                ConfigureStage(i, _intermediateStages[i - 2]);
            }

            SetLayerRecursively(this.gameObject, LayerMask.NameToLayer("Robot"));
        }

        private void ValidateIntermediateStages()
        {
            int intermediateCount = _numStages - 2;

            // Adds subparents when needed
            while (_intermediateStagesSubParents.Count < intermediateCount)
            {
                GameObject subParent = null;
                string name = _intermediateStageName + " " + (_intermediateStagesSubParents.Count + 1);
                ValidateDirectory(ref subParent, name, _intermediateStagesParent);
                InitializeConfigurableJoint(subParent);
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
                _intermediateStages.Add(new GameObject[2]);
            }


            while (_intermediateStages.Count > intermediateCount)
            {
                var arr = _intermediateStages[^1];
                if (arr != null)
                {
                    foreach (var go in arr)
                        if (go != null)
                            DestroyImmediate(go);
                }

                _intermediateStages.RemoveAt(_intermediateStages.Count - 1);
            }

            // Validate each intermediate stage's cubes under its subparent
            for (int i = 0; i < intermediateCount; i++)
            {
                var subParent = _intermediateStagesSubParents[i];
                string name = _intermediateStageName + (i + 1);
                _intermediateStages[i] = ValidateStage(_intermediateStages[i], subParent, name);
            }
        }

        // Cleans up duplicate logic, reduces nesting, and clarifies intent.
        private GameObject[] ValidateStage(GameObject[] referenceStage, GameObject parent, string name)
        {
            if (referenceStage == null || referenceStage.Length != 2)
                referenceStage = new GameObject[2];

            string[] suffixes = { " Left", " Right" };
            for (int i = 0; i < 2; i++)
            {
                string childName = name + suffixes[i];
                if (referenceStage[i] == null)
                {
                    Transform t = parent.transform.Find(childName);
                    if (t != null)
                    {
                        referenceStage[i] = t.gameObject;
                    }
                    else
                    {
                        referenceStage[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    }
                }

                referenceStage[i].name = childName;
                referenceStage[i].transform.SetParent(parent.transform, false);
            }

            return referenceStage;
        }

        private void UpdateElevatorOffset()
        {
            Vector3 positionOffset = new Vector3(
                _stageXOffset * _offsetUnitMultiplier,
                _stageYOffset * _offsetUnitMultiplier,
                _stageZOffset * _offsetUnitMultiplier);
            Quaternion rotationOffset = Quaternion.Euler(0f, _stageRotation, 0f);

            transform.SetLocalPositionAndRotation(positionOffset, rotationOffset);
        }

        private void UpdateElevatorMultipliers()
        {
            _tubingUnitMultiplier = RobotHelper.UnitToMeters(_tubingUnit);
            _offsetUnitMultiplier = RobotHelper.UnitToMeters(_offsetUnit);
            _baseStageUnitMultiplier = RobotHelper.UnitToMeters(_baseStageUnit);
        }

        /// <summary>
        /// Configure the joint chain so intermediates (if any) chain from the base stage up to the top.
        /// The base stage is explicitly excluded from having a ConfigurableJoint and must be kinematic.
        /// The lowest intermediate will connect to the base stage Rigidbody. Serialized _targetRigidbody is ignored.
        /// </summary>
        private void SetupStageJointChain()
        {
            // --- Ensure base stage has a kinematic Rigidbody and no ConfigurableJoint ---
            Rigidbody baseRb = _baseStageParent.GetOrAddComponent<Rigidbody>();
            baseRb.isKinematic = true;
            baseRb.useGravity = false;

            // --- Build chain of movable stages (intermediates lowest -> highest, then top) ---
            var chain = new List<GameObject>();
            if (_intermediateStagesSubParents != null && _intermediateStagesSubParents.Count > 0)
                chain.AddRange(_intermediateStagesSubParents);

            chain.Add(_topStageParent);

            Rigidbody previousRb = baseRb;
            // Connect each chain stage to the previous rigidbody in the chain.
            foreach (GameObject stageObj in chain)
            {
                if (stageObj == null) continue;

                ConfigurableJoint joint = stageObj.GetComponent<ConfigurableJoint>();
                Rigidbody rb = stageObj.GetComponent<Rigidbody>();

                // Connect this joint to the previous link (base or previous intermediate)
                joint.connectedBody = previousRb;

                // The next stage should connect to this stage's Rigidbody
                previousRb = rb;
            }
        }

        /// <summary>
        /// Scales and positions the left and right GameObjects for a given elevator stage.
        /// </summary>
        /// <param name="stageNumber">The stage number (1 = base, _numStages = top).</param>
        /// <param name="stageObjects">Array of two GameObjects: [0] = left, [1] = right.</param>
        private void ConfigureStage(int stageNumber, GameObject[] stageObjects)
        {
            // Replace the code in ConfigureStage to move the tubes along the X axis instead of Z
            if (stageObjects == null || stageObjects.Length != 2) return;

            // Calculate how many tubes inwards this stage is (0 = outermost/base, increases inward)
            int inwardIndex = stageNumber - 1;
            float width = _tubingWidth * _tubingUnitMultiplier;
            float height = _tubingHeight * _tubingUnitMultiplier;
            float length = _tubingLength * _tubingUnitMultiplier;
            float spacing = _tubingSpacing * _tubingUnitMultiplier;

            // Calculate Z offset for this stage (inward from base)
            float zOffset = inwardIndex * (width + spacing);

            // Calculate X positions for left and right tubes
            float baseStageWidth = _baseStageWidth * _baseStageUnitMultiplier;
            float leftX = -baseStageWidth / 2f + width / 2f + zOffset;
            float rightX = baseStageWidth / 2f - width / 2f - zOffset;

            // Y is always 0 for the base of the tube
            Vector3 leftPos = new Vector3(leftX, height / 2f, 0f);
            Vector3 rightPos = new Vector3(rightX, height / 2f, 0f);

            // Set scale and position for left tube
            if (stageObjects[0] != null)
            {
                stageObjects[0].transform.localScale = new Vector3(width, height, length);
                stageObjects[0].transform.localPosition = leftPos;
            }

            // Set scale and position for right tube
            if (stageObjects[1] != null)
            {
                stageObjects[1].transform.localScale = new Vector3(width, height, length);
                stageObjects[1].transform.localPosition = rightPos;
            }
        }

        /// <summary>
        /// Initializes a <see cref="ConfigurableJoint"/> on the specified stage object, configuring it for limited
        /// linear motion along the Y-axis and fully locked angular motion.
        /// </summary>
        /// <remarks>If the specified <paramref name="stage"/> does not already have a <see
        /// cref="Rigidbody"/>, one will be added to ensure the joint can operate. Similarly, if a <see
        /// cref="ConfigurableJoint"/> is not already present, a new one will be created. The joint is configured to
        /// allow limited sliding motion along the Y-axis, with the limit determined by the tubing height and unit
        /// multiplier. All other linear and angular motions are locked. The joint's connected body is set to the parent
        /// object's <see cref="Rigidbody"/>, if available.</remarks>
        /// <param name="stage">The <see cref="GameObject"/> representing the stage to which the joint will be added or configured. Must not
        /// be <see langword="null"/>.</param>
        private void InitializeConfigurableJoint(GameObject stage)
        {
            if (stage == null) return;

            // Reuse existing joint if present, otherwise create one.
            ConfigurableJoint joint = stage.GetComponent<ConfigurableJoint>();
            if (joint == null)
            {
                joint = stage.AddComponent<ConfigurableJoint>();
            }

            // Default connected body is left to SetupStageJointChain; keep parent fallback for editor clarity.
            Rigidbody parentRb = stage.transform.parent != null ? stage.transform.parent.GetComponent<Rigidbody>() : null;
            joint.connectedBody = parentRb;

            // Common joint basics
            joint.anchor = Vector3.zero;
            joint.axis = Vector3.forward;
            joint.secondaryAxis = Vector3.zero;

            // Linear motion: lock X and Z, allow limited Y (sliding).
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Locked;

            // Angular motion: fully locked (no rotation of stage body)
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            SoftJointLimit linearLimit = new()
            {
                limit = _tubingHeight * _tubingUnitMultiplier
            };
            joint.linearLimit = linearLimit;

            JointDrive yDrive = new()
            {
                positionSpring = 1000f,
                positionDamper = 100f,
                maximumForce = Mathf.Infinity
            };
            joint.yDrive = yDrive;
        }
        
        /// <summary>
        /// Configures and sets the stages for the cascading elevator subsystem.
        /// </summary>
        /// <remarks>This method initializes an array of stage GameObjects based on the number of stages
        /// and assigns the base stage, intermediate stages, and top stage to their respective positions. The configured
        /// stages are then passed to the cascading elevator subsystem for further processing.</remarks>
        private void SetSubsystemStages()
        {
            GameObject[] stages = new GameObject[_numStages];
            stages[0] = GameObject.Find(_baseStageParentName);

            for (int i = 1; i <= _numStages - 2; i++)
            {
                stages[i] = GameObject.Find(_intermediateStageName + " " + i);
            }
        
            stages[^1] = GameObject.Find(_topStageParentName);
        
            this.GetComponent<CascadingElevatorSubsystem>().SetCascadingElevatorGeneratorAndStages(stages);
        }
#endif
    }
}