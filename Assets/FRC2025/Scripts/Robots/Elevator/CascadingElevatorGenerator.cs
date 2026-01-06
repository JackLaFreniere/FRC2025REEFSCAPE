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

        /// <summary>
        /// Initializes the component and prepares it for use when the script instance is being loaded.
        /// </summary>
        /// <remarks>This method is called by Unity before any Start methods and before the game object is
        /// enabled. Override this method to perform setup tasks that need to occur once during the component's
        /// lifetime.</remarks>
        private void Awake()
        {
            _name = "Cascading Elevator Subsystem";
            _hasInitializedSubsystem = false;
        }

        /// <summary>
        /// Performs per-frame update logic for the component, handling both runtime and editor-specific updates as
        /// appropriate.
        /// </summary>
        /// <remarks>This method is called automatically by the Unity engine each frame. It distinguishes
        /// between play mode and edit mode, invoking the relevant update logic for each context. Override this method
        /// to extend or customize update behavior, but ensure to call the base implementation to maintain correct
        /// functionality.</remarks>
        protected override void Update()
        {
            base.Update(); // Generator.cs Update()

            if (Application.isPlaying) // Runtime update
            {
                HandleRunTimeUpdate();
                return;
            }

            HandleEditorUpdate(); // Editor update
        }

        /// <summary>
        /// Performs a runtime update by ensuring the subsystem is initialized and applying minimum position constraints
        /// to all runtime stages.
        /// </summary>
        /// <remarks>This method should be called during the runtime update cycle to maintain the correct
        /// state of the subsystem and its associated stages. It is intended for internal use within the update loop and
        /// is not thread-safe.</remarks>
        private void HandleRunTimeUpdate()
        {
            if (!_hasInitializedSubsystem)
            {
                InitializeSubsystem();
            }
            
            // Keep all stages at or above y = 0
            for (int i = 1; i < _numStages; i++)
            {
                ConstrainMinPosition(_runtimeStages[i]);
            }
        }

        /// <summary>
        /// Initializes the elevator subsystem and prepares it for operation.
        /// </summary>
        /// <remarks>This method configures the runtime stages and associates them with the elevator
        /// subsystem. It should be called before performing any operations that depend on the subsystem being
        /// initialized.</remarks>
        private void InitializeSubsystem()
        {
            _runtimeStages = GetStageGameObjects();
            GetComponent<CascadingElevatorSubsystem>().SetElevatorStages(_runtimeStages);
            _hasInitializedSubsystem = true;
        }

        /// <summary>
        /// Retrieves an array of GameObject instances representing each stage in the current configuration.
        /// </summary>
        /// <remarks>The returned array always has a length equal to the number of stages. The first
        /// element represents the base stage, intermediate elements represent intermediate stages (if any), and the
        /// last element represents the top stage. Callers should check for null values in the array if a stage
        /// GameObject might be missing in the scene.</remarks>
        /// <returns>An array of GameObject objects, where each element corresponds to a stage. The array is ordered from the
        /// base stage, through intermediate stages, to the top stage. Elements may be null if a stage GameObject is not
        /// found.</returns>
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

        /// <summary>
        /// Ensures that the specified stage object's local Y position is not less than zero.
        /// </summary>
        /// <remarks>This method modifies the localPosition of the stage object if its Y component is
        /// negative, setting it to zero. Other components of the position remain unchanged.</remarks>
        /// <param name="stage">The GameObject whose local Y position will be constrained. If null, the method performs no action.</param>
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

        /// <summary>
        /// Performs editor-time updates to ensure the stage hierarchy, configuration, and physics setup are current.
        /// </summary>
        /// <remarks>This method is intended to be called within the Unity Editor to synchronize stage
        /// components and maintain correct editor state. It should not be called at runtime.</remarks>
        private void HandleEditorUpdate()
        {
            UpdateUnitMultipliers(); // Update unit conversion factors
            ValidateStageHierarchy(); // Ensure all of the directories are valid
            ConfigureAllStages(); // Configure all of the objects in the stages
            SetupPhysicsJoints(); // Setup physics joints for all stages
            UpdateElevatorTransform(); // Update elevator position and rotation to offset values
        }
        
        /// <summary>
        /// Validates the configuration and hierarchy of all stage-related directories and stage tube assignments.
        /// </summary>
        /// <remarks>This method ensures that the base, top, and intermediate stage directories are
        /// correctly set up and that the associated stage tubes are valid. It should be called before performing
        /// operations that depend on a consistent stage hierarchy.</remarks>
        private void ValidateStageHierarchy()
        {
            // Validate parent directories
            ValidateDirectory(ref _baseStageParent, _baseStageParentName);
            ValidateDirectory(ref _topStageParent, _topStageParentName);
            ValidateDirectory(ref _intermediateStagesParent, _intermediateStagesParentName);

            // Validate base and top stages
            _baseStage = ValidateStageTubes(_baseStage, _baseStageParent, _baseStageParentName);
            _topStage = ValidateStageTubes(_topStage, _topStageParent, _topStageParentName);

            // Validate intermediate stages
            ValidateIntermediateStages();
        }

        /// <summary>
        /// Validates and synchronizes the intermediate stage subparents and stage tube arrays to match the current
        /// number of stages and tubes per stage.
        /// </summary>
        /// <remarks>This method ensures that the collections representing intermediate stages and their
        /// subparent GameObjects are correctly sized and updated based on the current configuration. It creates or
        /// removes GameObjects and arrays as needed to maintain consistency. This method should be called whenever the
        /// number of stages or tubes per stage changes to keep the scene hierarchy and data structures in
        /// sync.</remarks>
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

        /// <summary>
        /// Validates and initializes the array of stage tube GameObjects, ensuring each tube exists as a child of the
        /// specified parent and is named according to the provided base name.
        /// </summary>
        /// <remarks>If an element in the input array is null or missing, the method attempts to find a
        /// child GameObject of the parent with the expected name. If not found, a new primitive cube is created in its
        /// place. The returned array is guaranteed to have the expected number of tubes, each named and parented
        /// appropriately.</remarks>
        /// <param name="tubes">An array of GameObjects representing the stage tubes to validate or initialize. Can be null or partially
        /// populated.</param>
        /// <param name="parent">The parent GameObject under which the stage tubes should be found or created.</param>
        /// <param name="name">The base name used to identify and assign names to each stage tube GameObject.</param>
        /// <returns>An array of GameObjects representing the validated or newly created stage tubes, each correctly named and
        /// parented.</returns>
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
        
        /// <summary>
        /// Destroys all non-null GameObject instances in the specified array immediately.
        /// </summary>
        /// <remarks>This method uses DestroyImmediate, which destroys objects instantly and should
        /// typically be used only in editor scripts. Use with caution, as destroying objects immediately can have side
        /// effects if called during gameplay.</remarks>
        /// <param name="tubes">An array of GameObject instances to be destroyed. Elements that are null are ignored. If the array itself is
        /// null, no action is taken.</param>
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

        /// <summary>
        /// Configures all processing stages by initializing each stage with its corresponding settings.
        /// </summary>
        /// <remarks>This method should be called to ensure that all stages are properly configured before
        /// starting processing. It configures the base, intermediate, and top stages in sequence. Calling this method
        /// multiple times will reconfigure all stages.</remarks>
        private void ConfigureAllStages()
        {
            ConfigureStage(1, _baseStage);
            ConfigureStage(_numStages, _topStage);

            for (int i = 2; i < _numStages; i++)
            {
                ConfigureStage(i, _intermediateStages[i - 2]);
            }
        }

        /// <summary>
        /// Configures the positions and scales of tubes for a specific stage based on the provided stage number and
        /// associated GameObjects.
        /// </summary>
        /// <remarks>This method does not perform any configuration if the number of objects in
        /// stageObjects does not match the expected tube count for the stage. The method positions and scales the tubes
        /// according to the stage layout parameters.</remarks>
        /// <param name="stageNumber">The one-based index of the stage to configure. Must be greater than or equal to 1.</param>
        /// <param name="stageObjects">An array of GameObjects representing the tubes for the stage. The array must not be null and must contain
        /// the expected number of tube objects for the stage.</param>
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

        /// <summary>
        /// Configures the local scale and position of the specified tube GameObject.
        /// </summary>
        /// <param name="tube">The GameObject representing the tube to configure. If null, the method performs no action.</param>
        /// <param name="scale">The local scale to apply to the tube.</param>
        /// <param name="position">The local position to set for the tube.</param>
        private void ConfigureTube(GameObject tube, Vector3 scale, Vector3 position)
        {
            if (tube == null) return;

            tube.transform.localScale = scale;
            tube.transform.localPosition = position;
        }

        /// <summary>
        /// Updates the local position and rotation of the elevator based on the current stage offset and rotation
        /// values.
        /// </summary>
        /// <remarks>This method applies the configured stage offsets and rotations to the elevator's
        /// transform. It should be called whenever the stage position or orientation changes to ensure the elevator
        /// remains correctly aligned.</remarks>
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

        /// <summary>
        /// Updates the internal unit multipliers based on the current unit settings.
        /// </summary>
        /// <remarks>This method recalculates conversion factors used for unit-to-meter conversions. It
        /// should be called whenever the unit settings for tubing, offset, or base stage are changed to ensure that
        /// subsequent calculations use the correct multipliers.</remarks>
        private void UpdateUnitMultipliers()
        {
            _tubingUnitMultiplier = RobotHelper.UnitToMeters(_tubingUnit);
            _offsetUnitMultiplier = RobotHelper.UnitToMeters(_offsetUnit);
            _baseStageUnitMultiplier = RobotHelper.UnitToMeters(_baseStageUnit);
        }

        #endregion

        #region Physics Joint Setup

        /// <summary>
        /// Initializes and configures the physics joints required for the stage.
        /// </summary>
        /// <remarks>Call this method to set up all necessary physics joint connections before performing
        /// operations that depend on the stage's physical structure. This method should be invoked during the
        /// initialization phase of the stage lifecycle.</remarks>
        private void SetupPhysicsJoints()
        {
            SetupBaseStage();
            SetupStageJointChain();
        }

        /// <summary>
        /// Initializes the base stage by removing its ConfigurableJoint component and configuring its Rigidbody
        /// settings.
        /// </summary>
        /// <remarks>This method performs setup operations on the base stage object if it is present. It
        /// is intended to be called during initialization to ensure the base stage is correctly prepared for further
        /// processing.</remarks>
        private void SetupBaseStage()
        {
            if (_baseStageParent == null) return;

            // Ensure a Rigidbody exists on the base stage and make it kinematic (anchor)
            InitializedRigidBody(_baseStageParent, false);

            // Ensure target connected body exists if a parent object is assigned
            Rigidbody connectedRb = null;
            if (_parentObject != null)
            {
                connectedRb = _parentObject.GetComponent<Rigidbody>();
            }

            // Add or reuse a ConfigurableJoint on the base stage
            ConfigurableJoint joint = _baseStageParent.GetOrAddComponent<ConfigurableJoint>();
            joint.connectedBody = connectedRb;

            joint.anchor = Vector3.zero;
            joint.axis = Vector3.forward;
            joint.secondaryAxis = Vector3.zero;

            // Lock all linear motion
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            // Lock all angular motion
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            // Stability settings
            joint.projectionMode = JointProjectionMode.PositionAndRotation;
            joint.projectionDistance = 0.01f;
            joint.projectionAngle = 1f;
            joint.enableCollision = false;
            joint.enablePreprocessing = true;

            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;
        }

        /// <summary>
        /// Initializes and connects the stage joints in sequence, forming a joint chain between the base, intermediate,
        /// and top stage objects.
        /// </summary>
        /// <remarks>This method configures the joint connections for all movable stage objects, ensuring
        /// that each stage's joint is connected to the previous stage's Rigidbody. The method has no effect if the base
        /// stage parent is not assigned.</remarks>
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

        /// <summary>
        /// Initializes a Rigidbody component on the specified GameObject and configures its kinematic and gravity
        /// settings.
        /// </summary>
        /// <remarks>If the GameObject does not already have a Rigidbody component, one is added. The
        /// Rigidbody's useGravity property is always set to false by this method.</remarks>
        /// <param name="gameObject">The GameObject to which the Rigidbody component will be added or configured. Cannot be null.</param>
        /// <param name="kinematic">true to set the Rigidbody as kinematic; otherwise, false. The default is false.</param>
        private void InitializedRigidBody(GameObject gameObject, bool kinematic = false)
        {
            if (gameObject == null) return;
            Rigidbody rb = gameObject.GetOrAddComponent<Rigidbody>();
            rb.isKinematic = kinematic;
            rb.useGravity = false;
        }

        /// <summary>
        /// Initializes and configures a vertical slider joint on the specified stage object.
        /// </summary>
        /// <param name="stage">The GameObject representing the stage to which the joint will be added. Cannot be null.</param>
        private void InitializeStageJoint(GameObject stage)
        {
            if (stage == null) return;

            InitializedRigidBody(stage);

            ConfigurableJoint joint = stage.GetOrAddComponent<ConfigurableJoint>();

            ConfigureVerticalSliderJoint(joint);
        }

        /// <summary>
        /// Configures the specified joint as a vertical slider, allowing limited movement along the Y axis while
        /// locking all other motions.
        /// </summary>
        /// <remarks>This method sets the joint's anchor and axes, restricts motion to a limited range
        /// along the Y axis, and applies a spring drive for vertical movement. All angular and other linear motions are
        /// locked. The linear limit and drive parameters are determined by the current tubing height and unit
        /// multiplier. This configuration is suitable for scenarios where an object should only move vertically within
        /// a defined range.</remarks>
        /// <param name="joint">The ConfigurableJoint to configure as a vertical slider. Must not be null.</param>
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