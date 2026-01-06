using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class ArmGenerator : Generator<ArmSubsystem>
    {
#if UNITY_EDITOR

        #region Serialized Fields

        [Header("Arm Dimensions")]
        [SerializeField] private UnitType _armUnit = UnitType.Inches;
        [SerializeField, Min(0f)] private float _armLength = 12f;
        [SerializeField, Min(0f)] private float _armWidth = 1f;

        [Header("Arm Positioning")]
        [SerializeField] private UnitType _offsetUnit = UnitType.Meters;
        [SerializeField] private float _armXOffset = 0f;
        [SerializeField] private float _armYOffset = 0f;
        [SerializeField] private float _armZOffset = 0f;
        [SerializeField] private float _armXRotationOffset = 0f;
        [SerializeField] private float _armYRotationOffset = 0f;
        [SerializeField] private float _armZRotationOffset = 0f;

        [Header("Rotation Configuration")]
        [SerializeField] private AxisDirection _rotationAxis = AxisDirection.Y;
        [SerializeField] private bool _rotateAroundCenter = false;

        #endregion

        #region Private Fields

        private float _armUnitMultiplier;
        private float _offsetUnitMultiplier;

        private GameObject _armParent;
        private GameObject _arm;

        private Vector3 _offsetPos;

        private GameObject _runtimeArm;
        private bool _hasInitializedSubsystem;

        private readonly string _armParentName = "Arm";

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
            _name = "Arm Subsystem";
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

            // Keep the Arm at (0, 0, 0)
            //ConstrainMinPosition(_armParent);
        }

        /// <summary>
        /// Initializes the elevator subsystem and prepares it for operation.
        /// </summary>
        /// <remarks>This method configures the runtime stages and associates them with the elevator
        /// subsystem. It should be called before performing any operations that depend on the subsystem being
        /// initialized.</remarks>
        private void InitializeSubsystem()
        {
            _runtimeArm = GetArmGameObjects();
            GetComponent<ArmSubsystem>().SetArmGameObjects(_runtimeArm, _rotationAxis);
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
        private GameObject GetArmGameObjects()
        {
            return GameObject.Find(_armParentName);
        }

        /// <summary>
        /// Ensures that the specified stage object's local Y position is not less than zero.
        /// </summary>
        /// <remarks>This method modifies the localPosition of the stage object if its Y component is
        /// negative, setting it to zero. Other components of the position remain unchanged.</remarks>
        /// <param name="arm">The GameObject whose local Y position will be constrained. If null, the method performs no action.</param>
        private void ConstrainMinPosition(GameObject arm)
        {
            if (arm == null) return;

            arm.transform.position = _parentObject.transform.position;
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
            ValidateArmHierarchy(); // Ensure all of the directories are valid
            ConfigureArm(_arm); // Configure all of the objects in the stages
            SetupPhysicsJoints(); // Setup physics joints for all stages
            UpdateArmTransform(); // Update elevator position and rotation to offset values
        }

        /// <summary>
        /// Validates the configuration and hierarchy of all stage-related directories and stage tube assignments.
        /// </summary>
        /// <remarks>This method ensures that the base, top, and intermediate stage directories are
        /// correctly set up and that the associated stage tubes are valid. It should be called before performing
        /// operations that depend on a consistent stage hierarchy.</remarks>
        private void ValidateArmHierarchy()
        {
            // Validate parent directories
            ValidateDirectory(ref _armParent, _armParentName);

            // Validate base and top stages
            _arm = ValidateArm(_arm, _armParent, _armParentName);
        }

        /// <summary>
        /// Validates and initializes the array of stage tube GameObjects, ensuring each tube exists as a child of the
        /// specified parent and is named according to the provided base name.
        /// </summary>
        /// <remarks>If an element in the input array is null or missing, the method attempts to find a
        /// child GameObject of the parent with the expected name. If not found, a new primitive cube is created in its
        /// place. The returned array is guaranteed to have the expected number of tubes, each named and parented
        /// appropriately.</remarks>
        /// <param name="arm">An array of GameObjects representing the stage tubes to validate or initialize. Can be null or partially
        /// populated.</param>
        /// <param name="parent">The parent GameObject under which the stage tubes should be found or created.</param>
        /// <param name="name">The base name used to identify and assign names to each stage tube GameObject.</param>
        /// <returns>An array of GameObjects representing the validated or newly created stage tubes, each correctly named and
        /// parented.</returns>
        private GameObject ValidateArm(GameObject arm, GameObject parent, string name)
        {
            string armName = $"{name} Cylinder";
            if (arm == null)
            {
                Transform t = parent.transform.Find(armName);
                arm = t != null ? t.gameObject : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }

            arm.name = armName;
            arm.transform.SetParent(parent.transform, false);

            return arm;
        }

        #endregion

        #region Configuration Methods

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
        private void ConfigureArm(GameObject arm)
        {
            if (arm == null) return;

            float width = _armWidth * _armUnitMultiplier;
            float length = _armLength * _armUnitMultiplier;

            float yOffset = _rotateAroundCenter ? 0f : length;
            Vector3 scale = new(width, length, width);
            Vector3 position = new(0f, yOffset, 0f);

            arm.transform.localScale = scale;
            arm.transform.localPosition = position;
        }

        /// <summary>
        /// Updates the local position and rotation of the elevator based on the current stage offset and rotation
        /// values.
        /// </summary>
        /// <remarks>This method applies the configured stage offsets and rotations to the elevator's
        /// transform. It should be called whenever the stage position or orientation changes to ensure the elevator
        /// remains correctly aligned.</remarks>
        private void UpdateArmTransform()
        {
            Vector3 positionOffset = new(
                _armXOffset * _offsetUnitMultiplier,
                _armYOffset * _offsetUnitMultiplier,
                _armZOffset * _offsetUnitMultiplier);

            Quaternion rotationOffset = Quaternion.Euler(
                _armXRotationOffset,
                _armYRotationOffset,
                _armZRotationOffset);

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
            _armUnitMultiplier = RobotHelper.UnitToMeters(_armUnit);
            _offsetUnitMultiplier = RobotHelper.UnitToMeters(_offsetUnit);
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
            //SetupStageJointChain();
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
            if (_armParent == null) return;

            InitializeStageJoint(_armParent);
        }

        /// <summary>
        /// Initializes and connects the stage joints in sequence, forming a joint chain between the base, intermediate,
        /// and top stage objects.
        /// </summary>
        /// <remarks>This method configures the joint connections for all movable stage objects, ensuring
        /// that each stage's joint is connected to the previous stage's Rigidbody. The method has no effect if the base
        /// stage parent is not assigned.</remarks>
        //private void SetupStageJointChain()
        //{
        //    if (_baseStageParent == null) return;

        //    Rigidbody baseRb = _baseStageParent.GetComponent<Rigidbody>();

        //    List<GameObject> movableStages = new();
        //    if (_intermediateStagesSubParents != null && _intermediateStagesSubParents.Count > 0)
        //        movableStages.AddRange(_intermediateStagesSubParents);

        //    if (_topStageParent != null)
        //        movableStages.Add(_topStageParent);

        //    Rigidbody previousRb = baseRb;
        //    foreach (GameObject stage in movableStages)
        //    {
        //        if (stage == null) continue;

        //        InitializeStageJoint(stage);

        //        ConfigurableJoint joint = stage.GetComponent<ConfigurableJoint>();
        //        Rigidbody rb = stage.GetComponent<Rigidbody>();

        //        if (joint != null && rb != null)
        //        {
        //            joint.connectedBody = previousRb;
        //            previousRb = rb;
        //        }
        //    }
        //}

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
            rb.mass = 0.5f;
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
            if (_parentObject != null)
            {
                joint.connectedBody = _parentObject.GetComponent<Rigidbody>();
            }

            joint.anchor = Vector3.zero;
            joint.axis = Vector3.forward;
            joint.secondaryAxis = Vector3.zero;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = _rotationAxis == AxisDirection.X ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
            joint.angularYMotion = _rotationAxis == AxisDirection.Y ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
            joint.angularZMotion = _rotationAxis == AxisDirection.Z ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;

            float armSpring = 50000f;
            float armDamper = 300f;

            joint.angularXDrive = new JointDrive
            {
                positionSpring = armSpring,
                positionDamper = armDamper,
                maximumForce = Mathf.Infinity
            };

            joint.angularYZDrive = new JointDrive
            {
                positionSpring = armSpring,
                positionDamper = armDamper,
                maximumForce = Mathf.Infinity
            };

            joint.slerpDrive = new JointDrive
            {
                positionSpring = armSpring,
                positionDamper = armDamper,
                maximumForce = Mathf.Infinity
            };
        }

        #endregion

#endif
    }
}