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

        [Header("Physics Configuration")]
        [SerializeField] private float _rotationSpring = 50000f;
        [SerializeField] private float _rotationDamper = 300f;
        [SerializeField] private float _rbMass = 1f;

        #endregion

        #region Private Fields

        private float _armUnitMultiplier;
        private float _offsetUnitMultiplier;

        private GameObject _armParent;
        private GameObject _arm;

        private GameObject _runtimeArm;
        private bool _hasInitializedSubsystem;

        private readonly string _armParentName = "Arm";

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Initializes the component and prepares it for use when the script instance is being loaded.
        /// </summary>
        /// <remarks>This method is called automatically by Unity as part of the MonoBehaviour lifecycle.
        /// It is typically used to perform initialization tasks before the game starts or the object becomes
        /// active.</remarks>
        private void Awake()
        {
            _name = "Arm Subsystem";
            _hasInitializedSubsystem = false;
        }

        /// <summary>
        /// Updates the generator's state for the current frame, handling both runtime and editor-specific logic.
        /// </summary>
        /// <remarks>This method is called once per frame by the Unity engine. When the application is
        /// running, it processes runtime updates; otherwise, it performs editor-specific updates. Overrides the base
        /// <see cref="Generator.Update"/> method to provide custom update behavior.</remarks>
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
        /// Ensures that the subsystem is initialized before processing runtime updates.
        /// </summary>
        /// <remarks>This method checks whether the subsystem has been initialized and performs
        /// initialization if necessary. It should be called before executing operations that depend on the subsystem
        /// being ready.</remarks>
        private void HandleRunTimeUpdate()
        {
            if (!_hasInitializedSubsystem)
            {
                InitializeSubsystem();
            }
        }

        /// <summary>
        /// Initializes the arm subsystem and prepares it for operation.
        /// </summary>
        /// <remarks>This method must be called before using any functionality that depends on the arm
        /// subsystem. Subsequent calls have no effect if the subsystem is already initialized.</remarks>
        private void InitializeSubsystem()
        {
            _runtimeArm = GetArmGameObjects();
            GetComponent<ArmSubsystem>().SetArmGameObjects(_runtimeArm, _rotationAxis);
            _hasInitializedSubsystem = true;
        }

        /// <summary>
        /// Retrieves the parent <see cref="GameObject"/> that contains the arm components.
        /// </summary>
        /// <returns>The <see cref="GameObject"/> representing the parent of the arm components, or <see langword="null"/> if no
        /// such object is found in the scene.</returns>
        private GameObject GetArmGameObjects()
        {
            return GameObject.Find(_armParentName);
        }

        #endregion

        #region Editor Methods

        /// <summary>
        /// Updates the editor state of the arm component to reflect the latest configuration and hierarchy changes.
        /// </summary>
        /// <remarks>This method should be called during editor updates to ensure that the arm's
        /// properties, hierarchy, and physics joints remain consistent with the current settings. It is intended for
        /// use within the Unity Editor and does not affect runtime behavior.</remarks>
        private void HandleEditorUpdate()
        {
            UpdateUnitMultipliers();
            ValidateArmHierarchy();
            ConfigureArm();
            SetupPhysicsJoints();
            UpdateArmTransform();
        }

        /// <summary>
        /// Validates the current ARM directory hierarchy and updates internal references to ensure consistency.
        /// </summary>
        /// <remarks>This method checks the validity of the parent directory and the ARM directory
        /// structure. It updates internal fields to reflect any changes or corrections found during validation. This
        /// method is intended for internal use and should be called whenever the ARM hierarchy may have
        /// changed.</remarks>
        private void ValidateArmHierarchy()
        {
            // Validate parent directories
            ValidateDirectory(ref _armParent, _armParentName);

            // Validate base and top stages
            _arm = ValidateArm(_arm, _armParent, _armParentName);
        }

        /// <summary>
        /// Ensures that a child <see cref="GameObject"/> representing an arm exists under the specified parent,
        /// creating it if necessary, and sets its name and parent accordingly.
        /// </summary>
        /// <param name="arm">The existing arm <see cref="GameObject"/>, or <see langword="null"/> to create a new one if not found under
        /// the parent.</param>
        /// <param name="parent">The parent <see cref="GameObject"/> under which the arm should be located or created. Cannot be <see
        /// langword="null"/>.</param>
        /// <param name="name">The base name used to identify or create the arm. The final arm name will be "<paramref name="name"/>
        /// Cylinder". Cannot be <see langword="null"/> or empty.</param>
        /// <returns>The <see cref="GameObject"/> representing the arm, either the existing one found under the parent or a newly
        /// created cylinder primitive with the specified name.</returns>
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
        /// Configures the arm's local scale and position based on the current arm dimensions and rotation settings.
        /// </summary>
        /// <remarks>This method updates the arm's transform to reflect the current width, length, and
        /// rotation center configuration. It has no effect if the arm object is not assigned.</remarks>
        private void ConfigureArm()
        {
            if (_arm == null) return;

            float width = _armWidth * _armUnitMultiplier;
            float length = _armLength * _armUnitMultiplier;

            float yOffset = _rotateAroundCenter ? 0f : length;
            Vector3 scale = new(width, length, width);
            Vector3 position = new(0f, yOffset, 0f);

            _arm.transform.localScale = scale;
            _arm.transform.localPosition = position;
        }

        /// <summary>
        /// Updates the local position and rotation of the arm transform based on the current offset and rotation
        /// values.
        /// </summary>
        /// <remarks>This method applies the configured position and rotation offsets to the arm's
        /// transform. It should be called whenever the offset or rotation values change to ensure the transform
        /// reflects the latest configuration.</remarks>
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
        /// Updates the internal unit multipliers for the arm and offset based on their current unit settings.
        /// </summary>
        /// <remarks>This method recalculates the conversion factors used to translate arm and offset
        /// measurements to meters.  Call this method after changing the unit settings to ensure that subsequent
        /// calculations use the correct multipliers.</remarks>
        private void UpdateUnitMultipliers()
        {
            _armUnitMultiplier = RobotHelper.UnitToMeters(_armUnit);
            _offsetUnitMultiplier = RobotHelper.UnitToMeters(_offsetUnit);
        }

        #endregion

        #region Physics Joint Setup

        /// <summary>
        /// Configures the physics joints required for the current stage.
        /// </summary>
        /// <remarks>This method should be called during the initialization phase to ensure that all
        /// necessary physics joints are properly set up before the stage is used. It is intended for internal use and
        /// is not designed to be called directly by external code.</remarks>
        private void SetupPhysicsJoints()
        {
            SetupBaseStage();
        }

        /// <summary>
        /// Initializes the base stage of the arm if a parent arm is present.
        /// </summary>
        /// <remarks>This method should be called to ensure the base stage is properly set up before
        /// performing operations that depend on the arm's initialization. If the parent arm is not set, the method
        /// performs no action.</remarks>
        private void SetupBaseStage()
        {
            if (_armParent == null) return;

            InitializeStageJoint(_armParent);
        }

        /// <summary>
        /// Initializes a <see cref="Rigidbody"/> component on the specified <see cref="GameObject"/> with predefined
        /// settings.
        /// </summary>
        /// <remarks>If the <paramref name="gameObject"/> does not already have a <see cref="Rigidbody"/>
        /// component, one is added. The method sets the mass to a predefined value, disables gravity, and configures
        /// the kinematic state as specified.</remarks>
        /// <param name="gameObject">The <see cref="GameObject"/> to which the <see cref="Rigidbody"/> will be added or configured. Cannot be
        /// <see langword="null"/>.</param>
        /// <param name="kinematic"><see langword="true"/> to set the <see cref="Rigidbody"/> as kinematic; otherwise, <see langword="false"/>.</param>
        private void InitializedRigidBody(GameObject gameObject, bool kinematic = false)
        {
            if (gameObject == null) return;
            Rigidbody rb = gameObject.GetOrAddComponent<Rigidbody>();
            rb.mass = _rbMass;
            rb.isKinematic = kinematic;
            rb.useGravity = false;
        }

        /// <summary>
        /// Initializes the stage joint by attaching and configuring a <see cref="ConfigurableJoint"/> component to the
        /// specified stage object.
        /// </summary>
        /// <param name="stage">The <see cref="GameObject"/> representing the stage to which the joint will be added and configured. Cannot
        /// be <see langword="null"/>.</param>
        private void InitializeStageJoint(GameObject stage)
        {
            if (stage == null) return;

            InitializedRigidBody(stage);

            ConfigurableJoint joint = stage.GetOrAddComponent<ConfigurableJoint>();

            ConfigureVerticalSliderJoint(joint);
        }

        /// <summary>
        /// Configures the specified <see cref="ConfigurableJoint"/> to behave as a vertical slider joint with rotation
        /// enabled on a single axis.
        /// </summary>
        /// <remarks>This method locks all linear motions and enables free rotation only on the axis
        /// specified by the current <c>_rotationAxis</c> field. The joint's angular drives are set using the configured
        /// spring and damper values. If a parent object is assigned, the joint is connected to its <see
        /// cref="Rigidbody"/>.</remarks>
        /// <param name="joint">The <see cref="ConfigurableJoint"/> to configure. Must not be <see langword="null"/>.</param>
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

            joint.angularXDrive = new JointDrive
            {
                positionSpring = _rotationSpring,
                positionDamper = _rotationDamper,
                maximumForce = Mathf.Infinity
            };

            joint.angularYZDrive = new JointDrive
            {
                positionSpring = _rotationSpring,
                positionDamper = _rotationDamper,
                maximumForce = Mathf.Infinity
            };

            joint.slerpDrive = new JointDrive
            {
                positionSpring = _rotationSpring,
                positionDamper = _rotationDamper,
                maximumForce = Mathf.Infinity
            };
        }

        #endregion

#endif
    }
}