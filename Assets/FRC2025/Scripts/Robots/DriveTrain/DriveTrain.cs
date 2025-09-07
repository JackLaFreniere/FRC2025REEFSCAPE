using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public abstract class DriveTrain : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Drive Train Settings")]
        [SerializeField] protected UnitType _robotPerimeterUnit = UnitType.Meters;
        [SerializeField, Min(0)] protected float _length = 1f;
        [SerializeField, Min(0)] protected float _width = 1f;

        [Header("Drive Rail Settings")]
        [SerializeField] protected UnitType _driveRailUnit = UnitType.Inches;
        [SerializeField] protected float _driveRailHeight = 2f;
        [SerializeField] protected float _driveRailWidth = 1f;

        [Header("Belly Pan Settings")]
        [SerializeField] protected bool _hasBellyPan = true;
        [SerializeField] protected UnitType _bellyPanUnit = UnitType.Inches;
        [SerializeField, Min(0)] protected float _bellyPanThickness = 1f;

        protected Rigidbody _rigidbody;

        protected bool _isInitialized = false;

        protected float _robotPerimeterMultiplier;
        protected float _driveRailSizeMultiplier;
        protected float _bellyPanThicknessMultiplier;

        private GameObject _driveRailsParent;
        private GameObject _bumpersParent;
        private GameObject _bellyPanParent;

        private GameObject _frontDriveRail;
        private GameObject _backDriveRail;
        private GameObject _leftDriveRail;
        private GameObject _rightDriveRail;

        private GameObject _frontBumperEdge;
        private GameObject _backBumperEdge;
        private GameObject _leftBumperEdge;
        private GameObject _rightBumperEdge;
        private GameObject _frontLeftBumperCorner;
        private GameObject _frontRightBumperCorner;
        private GameObject _backLeftBumperCorner;
        private GameObject _backRightBumperCorner;

        private GameObject _bellyPan;

        protected string _driveTrainName;

        private readonly string _driveRailsName = "Drive Rails";
        private readonly string _bumpersName = "Bumpers";
        private readonly string _bumperEdgeName = "BumperEdge";
        private readonly string _bumperCornerName = "BumperCorner";
        private readonly string _frontDriveRailName = "F_DR"; // Front Drive Rail
        private readonly string _backDriveRailName = "B_DR"; // Back Drive Rail
        private readonly string _leftDriveRailName = "L_DR"; // Left Drive Rail
        private readonly string _rightDriveRailName = "R_DR"; // Right Drive Rail
        private readonly string _frontBumperEdgeName = "F_BE"; // Front Bumper Edge
        private readonly string _backBumperEdgeName = "B_BE"; // Back Bumper Edge
        private readonly string _leftBumperEdgeName = "L_BE"; // Left Bumper Edge
        private readonly string _rightBumperEdgeName = "R_BE"; // Right Bumper Edge
        private readonly string _frontLeftBumperCornerName = "FL_BC"; // Front Left Bumper Corner
        private readonly string _frontRightBumperCornerName = "FR_BC"; // Front Right Bumper Corner
        private readonly string _backLeftBumperCornerName = "BL_BC"; // Back Left Bumper Corner
        private readonly string _backRightBumperCornerName = "BR_BC"; // Back Right Bumper Corner
        private readonly string _bellyPanName = "Belly Pan";

        /// <summary>
        /// Updates the configuration, positioning, and scaling of the robot's structural components, including drive
        /// rails, bumpers, and the belly pan.
        /// </summary>
        /// <remarks>This method ensures that all required parent objects and components are initialized
        /// and properly positioned and scaled based on the current dimensions and multipliers. It is intended to be
        /// used in the Unity Editor and will not execute during runtime.</remarks>
        protected virtual void Update()
        {
            if (!_isInitialized) return;

            UpdateDriveTrainMultipliers();

            // Ensure parent objects exist for organizational purposes
            ValidateDirectory(ref _driveRailsParent, _driveRailsName);
            ValidateDirectory(ref _bumpersParent, _bumpersName);
            ValidateDirectory(ref _bellyPanParent, _bellyPanName);

            //ValidateRB(ref _rigidbody);

            // Create the drive rails and bumpers if they don't exist
            InitializeDriveRails();
            InitializeBumpers();
            InitializeBellyPan();

            // Position and scale each drive rail
            _frontDriveRail.transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, (_length / 2f) * _robotPerimeterMultiplier - (_driveRailWidth / 2f) * _driveRailSizeMultiplier), Quaternion.identity);
            _backDriveRail.transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, (-_length / 2f) * _robotPerimeterMultiplier + (_driveRailWidth / 2f) * _driveRailSizeMultiplier), Quaternion.identity);
            _leftDriveRail.transform.SetLocalPositionAndRotation(new Vector3((-_width / 2f) * _robotPerimeterMultiplier + (_driveRailWidth / 2f) * _driveRailSizeMultiplier, 0f, 0f), Quaternion.identity);
            _rightDriveRail.transform.SetLocalPositionAndRotation(new Vector3((_width / 2f) * _robotPerimeterMultiplier - (_driveRailWidth / 2f) * _driveRailSizeMultiplier, 0f, 0f), Quaternion.identity);

            _frontDriveRail.transform.localScale = new Vector3(_width * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f, _driveRailHeight * _driveRailSizeMultiplier, _driveRailWidth * _driveRailSizeMultiplier);
            _backDriveRail.transform.localScale = new Vector3(_width * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f, _driveRailHeight * _driveRailSizeMultiplier, _driveRailWidth * _driveRailSizeMultiplier);
            _leftDriveRail.transform.localScale = new Vector3(_driveRailWidth * _driveRailSizeMultiplier, _driveRailHeight * _driveRailSizeMultiplier, _length * _robotPerimeterMultiplier);
            _rightDriveRail.transform.localScale = new Vector3(_driveRailWidth * _driveRailSizeMultiplier, _driveRailHeight * _driveRailSizeMultiplier, _length * _robotPerimeterMultiplier);

            // Position and scale each bumper edge and corner
            _frontBumperEdge.transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, (_length / 2f) * _robotPerimeterMultiplier), Quaternion.identity);
            _backBumperEdge.transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, (-_length / 2f) * _robotPerimeterMultiplier), Quaternion.Euler(0f, 180f, 0f));
            _leftBumperEdge.transform.SetLocalPositionAndRotation(new Vector3((-_width / 2f) * _robotPerimeterMultiplier, 0f, 0f), Quaternion.Euler(0f, -90f, 0f));
            _rightBumperEdge.transform.SetLocalPositionAndRotation(new Vector3((_width / 2f) * _robotPerimeterMultiplier, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));

            _frontBumperEdge.transform.localScale = new Vector3(_width * _robotPerimeterMultiplier, 1f, 1f);
            _backBumperEdge.transform.localScale = new Vector3(_width * _robotPerimeterMultiplier, 1f, 1f);
            _leftBumperEdge.transform.localScale = new Vector3(_length * _robotPerimeterMultiplier, 1f, 1f);
            _rightBumperEdge.transform.localScale = new Vector3(_length * _robotPerimeterMultiplier, 1f, 1f);

            _frontRightBumperCorner.transform.SetLocalPositionAndRotation(new Vector3((_width / 2f) * _robotPerimeterMultiplier, 0f, (_length / 2f) * _robotPerimeterMultiplier), Quaternion.Euler(0f, 90f, 0f));
            _frontLeftBumperCorner.transform.SetLocalPositionAndRotation(new Vector3((-_width / 2f) * _robotPerimeterMultiplier, 0f, (_length / 2f) * _robotPerimeterMultiplier), Quaternion.Euler(0f, 0f, 0f));
            _backRightBumperCorner.transform.SetLocalPositionAndRotation(new Vector3((_width / 2f) * _robotPerimeterMultiplier, 0f, (-_length / 2f) * _robotPerimeterMultiplier), Quaternion.Euler(0f, 180f, 0f));
            _backLeftBumperCorner.transform.SetLocalPositionAndRotation(new Vector3((-_width / 2f) * _robotPerimeterMultiplier, 0f, (-_length / 2f) * _robotPerimeterMultiplier), Quaternion.Euler(0f, -90f, 0f));

            _frontLeftBumperCorner.transform.localScale = Vector3.one;
            _frontRightBumperCorner.transform.localScale = Vector3.one;
            _backLeftBumperCorner.transform.localScale = Vector3.one;
            _backRightBumperCorner.transform.localScale = Vector3.one;

            // Position and scale the belly pan
            _bellyPan.transform.SetLocalPositionAndRotation(new Vector3(0f, (-_driveRailHeight * _driveRailSizeMultiplier / 2f) + (_bellyPanThickness * _bellyPanThicknessMultiplier / 2f), 0f), Quaternion.identity);

            _bellyPan.transform.localScale = new Vector3(_width * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f, _bellyPanThickness * _bellyPanThicknessMultiplier, _length * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f);
        }

        /// <summary>
        /// Ensures that the specified directory exists by validating or creating it.
        /// </summary>
        /// <remarks>If the directory is created, it is parented to the current object's transform, and
        /// its local position  and rotation are reset to <see cref="Vector3.zero"/> and <see
        /// cref="Quaternion.identity"/>, respectively.</remarks>
        /// <param name="directory">A reference to the <see cref="GameObject"/> representing the directory. If the directory is  <see
        /// langword="null"/>, it will be initialized to an existing child object with the specified name,  or a new
        /// <see cref="GameObject"/> will be created if no such child exists.</param>
        /// <param name="name">The name of the directory to validate or create. This name is used to search for an existing child  object
        /// or to assign to the newly created <see cref="GameObject"/>.</param>
        protected void ValidateDirectory(ref GameObject directory, string name)
        {
            if (directory != null) return;

            if (transform.Find(name) != null)
            {
                directory = transform.Find(name).gameObject;
                return;
            }

            directory = new(name)
            {
                name = name
            };
            directory.transform.SetParent(transform);
            directory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Ensures that the specified <see cref="Rigidbody"/> reference is valid by adding a new  <see
        /// cref="Rigidbody"/> component to the current GameObject if one does not already exist.
        /// </summary>
        /// <remarks>If the current GameObject does not already have a <see cref="Rigidbody"/> component,
        /// one will be added.  The provided <paramref name="rb"/> reference is not modified by this method.</remarks>
        /// <param name="rb">A reference to the <see cref="Rigidbody"/> to validate. This parameter is passed by reference.</param>
        protected void ValidateRB(ref Rigidbody rb)
        {
            if (rb != null) return;

            rb = this.GetComponent<Rigidbody>();
            if (rb != null) return;
            
            rb = this.gameObject.AddComponent<Rigidbody>();
        }

        /// <summary>
        /// Initializes and validates the drive rail components for the system.
        /// </summary>
        /// <remarks>This method ensures that all drive rails are properly validated and initialized 
        /// before use. Each drive rail is checked using the <see cref="ValidateDriveRails"/> method.</remarks>
        private void InitializeDriveRails()
        {
            _frontDriveRail = ValidateDriveRails(_frontDriveRail, _frontDriveRailName);
            _backDriveRail = ValidateDriveRails(_backDriveRail, _backDriveRailName);
            _leftDriveRail = ValidateDriveRails(_leftDriveRail, _leftDriveRailName);
            _rightDriveRail = ValidateDriveRails(_rightDriveRail, _rightDriveRailName);
        }

        /// <summary>
        /// Initializes and validates the bumper edges and corners for the object.
        /// </summary>
        /// <remarks>This method ensures that all bumper edges and corners are properly validated and
        /// initialized using the <c>ValidateBumpers</c> method. It processes both the front, back, left, and right
        /// bumper edges, as well as the front-left, front-right, back-left, and back-right bumper corners.</remarks>
        private void InitializeBumpers()
        {
            _frontBumperEdge = ValidateBumpers<BoxCollider>(_frontBumperEdge, _frontBumperEdgeName, _bumpersName, _bumperEdgeName);
            _backBumperEdge = ValidateBumpers<BoxCollider>(_backBumperEdge, _backBumperEdgeName, _bumpersName, _bumperEdgeName);
            _leftBumperEdge = ValidateBumpers<BoxCollider>(_leftBumperEdge, _leftBumperEdgeName, _bumpersName, _bumperEdgeName);
            _rightBumperEdge = ValidateBumpers<BoxCollider>(_rightBumperEdge, _rightBumperEdgeName, _bumpersName, _bumperEdgeName);

            _frontLeftBumperCorner = ValidateBumpers<CapsuleCollider>(_frontLeftBumperCorner, _frontLeftBumperCornerName, _bumpersName, _bumperCornerName);
            _frontRightBumperCorner = ValidateBumpers<CapsuleCollider>(_frontRightBumperCorner, _frontRightBumperCornerName, _bumpersName, _bumperCornerName);
            _backLeftBumperCorner = ValidateBumpers<CapsuleCollider>(_backLeftBumperCorner, _backLeftBumperCornerName, _bumpersName, _bumperCornerName);
            _backRightBumperCorner = ValidateBumpers<CapsuleCollider>(_backRightBumperCorner, _backRightBumperCornerName, _bumpersName, _bumperCornerName);
        }

        /// <summary>
        /// Initializes the belly pan by validating its configuration and setting its active state.
        /// </summary>
        /// <remarks>This method ensures that the belly pan is properly validated and updates its active
        /// state  based on the current configuration. The belly pan's state is determined by the value of  <see
        /// cref="_hasBellyPan"/>.</remarks>
        private void InitializeBellyPan()
        {
            _bellyPan = ValidateBellyPan(_bellyPan, _bellyPanName);

            _bellyPan.GetComponent<BoxCollider>().enabled = _hasBellyPan;
            _bellyPan.GetComponent<MeshRenderer>().enabled = _hasBellyPan;
        }

        /// <summary>
        /// Validates and retrieves a drive rail GameObject by name, creating a new one if it does not exist.
        /// </summary>
        /// <remarks>If the <paramref name="referenceDriveRail"/> is null and no existing GameObject with
        /// the specified <paramref name="name"/> is found, a new GameObject is created as a cube primitive, assigned
        /// the specified name, and added as a child of the drive rails parent. The BoxCollider component of the newly
        /// created GameObject is removed immediately.</remarks>
        /// <param name="referenceDriveRail">The existing drive rail GameObject to validate. If null, a new drive rail will be created.</param>
        /// <param name="name">The name of the drive rail to find or assign to the newly created GameObject.</param>
        /// <returns>The validated or newly created drive rail GameObject. If a GameObject with the specified name exists as a
        /// child of the drive rails parent, it is returned; otherwise, a new GameObject is created and returned.</returns>
        private GameObject ValidateDriveRails(GameObject referenceDriveRail, string name)
        {
            if (referenceDriveRail != null) return referenceDriveRail;

            Transform rail = _driveRailsParent.transform.Find(name);
            if (rail != null)
            {
                return rail.gameObject;
            }

            referenceDriveRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            referenceDriveRail.name = name;
            referenceDriveRail.transform.SetParent(_driveRailsParent.transform);

            return referenceDriveRail;
        }

        /// <summary>
        /// Validates and retrieves a bumper GameObject, creating and configuring it if it does not already exist.
        /// </summary>
        /// <remarks>If the specified bumper GameObject does not exist, this method will instantiate a new
        /// GameObject from the specified asset, assign it the specified name, parent it to the designated parent
        /// transform, and add the specified type of <see cref="Collider"/> to it.</remarks>
        /// <typeparam name="T">The type of <see cref="Collider"/> to add to the bumper GameObject if it needs to be created.</typeparam>
        /// <param name="referenceBumper">The existing bumper GameObject to validate. If not null, it will be returned as-is.</param>
        /// <param name="objName">The name of the bumper GameObject to search for under the parent transform.</param>
        /// <param name="subfolderDirectory">The relative path to the subfolder containing the asset to instantiate if the bumper does not exist.</param>
        /// <param name="assetName">The name of the asset to load and instantiate if the bumper does not exist.</param>
        /// <returns>The validated or newly created bumper GameObject. If the bumper already exists, it is returned as-is.
        /// Otherwise, a new bumper GameObject is instantiated, configured, and returned.</returns>
        private GameObject ValidateBumpers<T>(GameObject referenceBumper, string objName, string subfolderDirectory, string assetName) where T : Collider
        {
            if (referenceBumper != null) return referenceBumper;

            Transform bumper = _bumpersParent.transform.Find(objName);
            if (bumper != null)
            {
                return bumper.gameObject;
            }
            else
            {
                referenceBumper = Instantiate(Resources.Load<GameObject>(subfolderDirectory + "/" + assetName));
                referenceBumper.name = objName;
                referenceBumper.transform.SetParent(_bumpersParent.transform);
                referenceBumper.AddComponent<T>();

                return referenceBumper;
            }
        }

        /// <summary>
        /// Validates and retrieves a belly pan GameObject by name, or creates a new one if it does not exist.
        /// </summary>
        /// <remarks>If <paramref name="referenceBellyPan"/> is null, the method attempts to find a
        /// GameObject with the specified name under the belly pan parent transform. If no such GameObject exists, a new
        /// GameObject is created as a cube primitive, assigned the specified name, and parented to the belly pan parent
        /// transform.</remarks>
        /// <param name="referenceBellyPan">The reference GameObject to validate. If not null, it will be returned as is.</param>
        /// <param name="name">The name of the belly pan to find or create.</param>
        /// <returns>The validated or newly created belly pan GameObject. If <paramref name="referenceBellyPan"/> is null and no
        /// existing GameObject with the specified name is found, a new GameObject is created and returned.</returns>
        private GameObject ValidateBellyPan(GameObject referenceBellyPan, string name)
        {
            if (referenceBellyPan != null) return referenceBellyPan;

            Transform rail = _bellyPanParent.transform.Find(name);
            if (rail != null)
            {
                return rail.gameObject;
            }

            referenceBellyPan = GameObject.CreatePrimitive(PrimitiveType.Cube);
            referenceBellyPan.name = name;
            referenceBellyPan.transform.SetParent(_bellyPanParent.transform);

            return referenceBellyPan;
        }

        /// <summary>
        /// Updates the internal unit multipliers used for converting dimensions to meters.
        /// </summary>
        /// <remarks>This method recalculates and updates the conversion factors for the robot perimeter, 
        /// drive rail size, and belly pan thickness based on their respective units. The updated  multipliers are used
        /// internally for dimension calculations.</remarks>
        protected void UpdateDriveTrainMultipliers()
        {
            _robotPerimeterMultiplier = UnitToMeters(_robotPerimeterUnit);
            _driveRailSizeMultiplier = UnitToMeters(_driveRailUnit);
            _bellyPanThicknessMultiplier = UnitToMeters(_bellyPanUnit);
        }

        /// <summary>
        /// Converts a specified unit of measurement to its equivalent value in meters.
        /// </summary>
        /// <param name="unit">The unit of measurement to convert. Supported values include <see cref="UnitType.Meters"/>, <see
        /// cref="UnitType.Centimeters"/>, and <see cref="UnitType.Inches"/>.</param>
        /// <returns>The equivalent value in meters. Returns 1 meter for unsupported or unknown unit types.</returns>
        protected float UnitToMeters(UnitType unit) => unit switch
        {
            UnitType.Meters => 1f,
            UnitType.Centimeters => 0.01f,
            UnitType.Inches => 0.0254f,
            _ => 1f
        };

        /// <summary>
        /// Attempts to remove this component from the GameObject if it has no parent transform.
        /// </summary>
        /// <remarks>This method immediately destroys the component if the <see cref="Transform.parent"/>
        /// property is null. Use with caution, as <see cref="DestroyImmediate"/> is typically intended for editor use
        /// and may have unintended side effects in runtime scenarios.</remarks>
        protected void AttemptRemoveSelf(Component script)
        {
            if (transform.parent == null)
            {
                DestroyImmediate(script);
            }
        }

        /// <summary>
        /// Removes all child GameObjects of the current Transform.
        /// </summary>
        /// <remarks>This method immediately destroys all child GameObjects of the Transform associated
        /// with the current object. Use with caution, as this operation cannot be undone and will remove all child
        /// objects without confirmation.</remarks>
        private void Reset()
        {
            if (transform.parent == null)
            {
                GameObject child = new(_driveTrainName);
                child.transform.SetParent(transform, false);

                DriveTrain swerve = (DriveTrain) child.AddComponent(GetType());
                swerve._isInitialized = true;

                return;
            }

            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
#endif
    }
}