using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public abstract class DriveTrainGenerator<S> : Generator<S> where S : Component
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

        // Internal functionality
        protected Rigidbody _rigidbody;

        // Parent objects for organization
        protected GameObject _wheelsParent;
        private GameObject _driveRailsParent;
        private GameObject _bumpersParent;
        private GameObject _bellyPanParent;

        // Child objects
        private GameObject _bellyPan;
        private readonly GameObject[] _driveRails = new GameObject[4];
        private readonly GameObject[] _bumperEdges = new GameObject[4];
        private readonly GameObject[] _bumperCorners = new GameObject[4];
        protected GameObject[] _wheels;
        protected WheelCollider[] _wheelColliders;

        // Parent names 
        private readonly string _driveRailsName = "Drive Rails";
        private readonly string _bumpersName = "Bumpers";
        private readonly string _bellyPanName = "Belly Pan";

        // Asset names
        private readonly string _bumperEdgeName = "BumperEdge";
        private readonly string _bumperCornerName = "BumperCorner";

        // Child names
        private readonly string[] _driveRailnames = new string[]
        {
            "F_DR", // Front Drive Rail
            "B_DR", // Back Drive Rail
            "L_DR", // Left Drive Rail
            "R_DR"  // Right Drive Rail
        };

        private readonly string[] _bumperEdgeNames = new string[]
        {
            "F_BE", // Front Bumper Edge
            "B_BE", // Back Bumper Edge
            "L_BE", // Left Bumper Edge
            "R_BE"  // Right Bumper Edge
        };

        private readonly string[] _bumperCornerNames = new string[]
        {
            "FL_BC", // Front Left Bumper Corner
            "FR_BC", // Front Right Bumper Corner
            "BL_BC", // Back Left Bumper Corner
            "BR_BC"  // Back Right Bumper Corner
        };

        // Multipliers for unit conversion
        protected float _robotPerimeterMultiplier;
        protected float _driveRailSizeMultiplier;
        protected float _bellyPanThicknessMultiplier;

        /// <summary>
        /// Updates the state of the object and its associated components, ensuring that all necessary elements
        /// are initialized and their transforms are updated.
        /// </summary>
        /// <remarks>This method performs several operations to maintain the integrity of the object's
        /// structure: it validates and initializes parent directories and components, and updates the
        /// transforms of associated elements. It is intended to be called periodically to ensure the object
        /// remains in a consistent and updated state.</remarks>
        protected override void Update()
        {
            if (!_isInitialized) return;

            UpdateDriveTrainMultipliers();

            // Ensure parent objects exist for organizational purposes
            ValidateDirectory(ref _driveRailsParent, _driveRailsName);
            ValidateDirectory(ref _bumpersParent, _bumpersName);
            ValidateDirectory(ref _bellyPanParent, _bellyPanName);

            ValidateRB(ref _rigidbody);

            // Initialize components if they don't exist
            InitializeDriveRails();
            InitializeBumpers();
            InitializeBellyPan();

            // Update component transforms
            UpdateDriveRails();
            UpdateBumpers();
            UpdateBellyPan();
        }

        /// <summary>
        /// Ensures that the specified <see cref="Rigidbody"/> reference is not null by assigning it to an existing 
        /// <see cref="Rigidbody"/> component on the current GameObject or by adding a new one if none exists.
        /// </summary>
        /// <param name="rb">A reference to the <see cref="Rigidbody"/> to validate. If null, it will be assigned to an existing or
        /// newly added <see cref="Rigidbody"/> component on the current GameObject.</param>
        protected void ValidateRB(ref Rigidbody rb)
        {
            if (rb != null) return;

            rb = this.GetComponent<Rigidbody>();
            if (rb != null) return;
            
            rb = this.gameObject.AddComponent<Rigidbody>();
        }

        /// <summary>
        /// Initializes the drive rails by validating and updating each rail in the collection.
        /// </summary>
        /// <remarks>This method iterates through the collection of drive rails and validates each one 
        /// using the associated name. The validation process ensures that the drive rails meet  the required criteria
        /// before being updated.</remarks>
        private void InitializeDriveRails()
        {
            for (int i = 0; i < _driveRails.Length; i++)
            {
                _driveRails[i] = ValidateDriveRails(_driveRails[i], _driveRailnames[i]);
            }
        }

        /// <summary>
        /// Initializes the bumpers by validating and assigning the appropriate colliders for edges and corners.
        /// </summary>
        /// <remarks>This method ensures that each bumper edge and corner is properly validated and
        /// assigned a collider of the expected type. It processes all elements in the bumper edges and corners arrays,
        /// ensuring that they are correctly configured for further use.</remarks>
        private void InitializeBumpers()
        {
            for (int i = 0; i < _bumperEdges.Length; i++)
            {
                _bumperEdges[i] = ValidateBumpers<BoxCollider>(_bumperEdges[i], _bumperEdgeNames[i], _bumpersName, _bumperEdgeName);
                _bumperCorners[i] = ValidateBumpers<CapsuleCollider>(_bumperCorners[i], _bumperCornerNames[i], _bumpersName, _bumperCornerName);
            }
        }

        /// <summary>
        /// Initializes the belly pan by validating its configuration and enabling or disabling its components based on
        /// the current state.
        /// </summary>
        /// <remarks>This method ensures that the belly pan is properly validated and updates its collider
        /// and renderer components according to the value of the <c>_hasBellyPan</c> field. If <c>_hasBellyPan</c> is 
        /// <see langword="true"/>, the components are enabled; otherwise, they are disabled.</remarks>
        private void InitializeBellyPan()
        {
            _bellyPan = ValidateBellyPan(_bellyPan, _bellyPanName);

            _bellyPan.GetComponent<BoxCollider>().enabled = _hasBellyPan;
            _bellyPan.GetComponent<MeshRenderer>().enabled = _hasBellyPan;
        }

        /// <summary>
        /// Validates and retrieves a drive rail GameObject by name, creating a new one if it does not exist.
        /// </summary>
        /// <remarks>If <paramref name="referenceDriveRail"/> is <see langword="null"/>, the method
        /// attempts to find a child GameObject with the specified <paramref name="name"/>  under the parent object. If
        /// no such GameObject exists, a new GameObject is created as a cube primitive, assigned the specified name, and
        /// parented to the drive rails parent.</remarks>
        /// <param name="referenceDriveRail">The existing drive rail GameObject to validate. Can be <see langword="null"/>.</param>
        /// <param name="name">The name of the drive rail to find or create.</param>
        /// <returns>The validated or newly created drive rail GameObject. If <paramref name="referenceDriveRail"/> is not <see
        /// langword="null"/>, it is returned as-is.</returns>
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
        /// Validates and retrieves a bumper GameObject, creating and initializing it if it does not already exist.
        /// </summary>
        /// <remarks>If the specified bumper GameObject does not exist, this method creates a new instance
        /// of the bumper asset from the Resources folder, assigns it the specified name, sets its parent to the
        /// designated bumpers parent transform, and adds the specified type of <see cref="Collider"/> to it.</remarks>
        /// <typeparam name="T">The type of <see cref="Collider"/> to add to the bumper GameObject if it needs to be created.</typeparam>
        /// <param name="referenceBumper">The existing bumper GameObject to validate. If not null, this object is returned as-is.</param>
        /// <param name="objName">The name of the bumper GameObject to search for under the parent transform.</param>
        /// <param name="subfolderDirectory">The directory path within the Resources folder where the bumper asset is located.</param>
        /// <param name="assetName">The name of the asset file to load if the bumper GameObject needs to be created.</param>
        /// <returns>The validated or newly created bumper GameObject. If the bumper already exists, it is returned directly.
        /// Otherwise, a new bumper is instantiated, initialized, and returned.</returns>
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
        /// Validates and retrieves a belly pan GameObject based on the provided reference or name.
        /// </summary>
        /// <param name="referenceBellyPan">The existing belly pan GameObject to validate. If this parameter is not null, it is returned as-is.</param>
        /// <param name="name">The name of the belly pan to search for under the parent transform. If no matching GameObject is found, a
        /// new GameObject with this name is created.</param>
        /// <returns>The validated or newly created belly pan GameObject. If <paramref name="referenceBellyPan"/> is not null, it
        /// is returned. Otherwise, the method searches for a GameObject with the specified <paramref name="name"/>
        /// under the parent transform. If no such GameObject exists, a new GameObject is created, named, and returned.</returns>
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
        /// Updates the internal multipliers for the robot's drive train dimensions based on the current unit settings.
        /// </summary>
        /// <remarks>This method recalculates and updates the multipliers used for the robot perimeter,
        /// drive rail size, and belly pan thickness. The updated values are derived by converting the respective unit
        /// settings to meters.</remarks>
        protected void UpdateDriveTrainMultipliers()
        {
            _robotPerimeterMultiplier = RobotHelper.UnitToMeters(_robotPerimeterUnit);
            _driveRailSizeMultiplier = RobotHelper.UnitToMeters(_driveRailUnit);
            _bellyPanThicknessMultiplier = RobotHelper.UnitToMeters(_bellyPanUnit);
        }

        /// <summary>
        /// Updates the position and scale of the drive rails.
        /// </summary>
        /// <remarks>This method adjusts the drive rails by updating their position and scale. It is
        /// intended to ensure the drive rails are correctly aligned and sized based on the current state of the
        /// system.</remarks>
        private void UpdateDriveRails()
        {
            UpdateDriveRailPosition();
            UpdateDriveRailScale();
        }

        /// <summary>
        /// Updates the positions and orientations of the drive rails based on the current dimensions and configuration
        /// of the robot.
        /// </summary>
        /// <remarks>This method calculates the offsets for each drive rail using the robot's width,
        /// length, and scaling multipliers. The calculated positions are applied to the drive rails to ensure they
        /// are correctly aligned relative to the robot's perimeter. The orientation of the drive rails is reset to the
        /// default identity rotation.</remarks>
        private void UpdateDriveRailPosition()
        {
            float xOffset = (_width / 2f) * _robotPerimeterMultiplier - (_driveRailWidth / 2f) * _driveRailSizeMultiplier;
            float yOffset = 0f;
            float zOffset = (_length / 2f) * _robotPerimeterMultiplier - (_driveRailWidth / 2f) * _driveRailSizeMultiplier;
            Quaternion driveRailEulerOffset = Quaternion.identity;

            Vector3[] driveRailPositionOffsets =
            {
                new (0f, yOffset,  zOffset),    // Front
                new (0f, yOffset, -zOffset),    // Back
                new (-xOffset, yOffset, 0f),    // Left
                new (xOffset,  yOffset, 0f),    // Right
            };

            for (int i = 0; i < _driveRails.Length; i++)
            {
                _driveRails[i].transform.SetLocalPositionAndRotation(driveRailPositionOffsets[i], driveRailEulerOffset);
            }
        }

        /// <summary>
        /// Updates the scale of the drive rails based on the current dimensions and multipliers.
        /// </summary>
        /// <remarks>This method calculates the scale for each drive rail using the configured height,
        /// width, and length dimensions, along with their respective multipliers. The calculated scales are then
        /// applied to the local scale of each drive rail in the array.</remarks>
        private void UpdateDriveRailScale()
        {
            float driveRailHeightScale = _driveRailHeight * _driveRailSizeMultiplier;
            float driveRailWidthScale = _driveRailWidth * _driveRailSizeMultiplier;
            float robotWidthScale = _width * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f + _driveRailWidth * _driveRailSizeMultiplier * 2f;
            float robotLengthScale = _length * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f;

            Vector3[] driveRailScaleOffsets =
            {
                new (robotWidthScale, driveRailHeightScale, driveRailWidthScale),   // Front
                new (robotWidthScale, driveRailHeightScale, driveRailWidthScale),   // Back
                new (driveRailWidthScale, driveRailHeightScale, robotLengthScale),  // Left
                new (driveRailWidthScale, driveRailHeightScale, robotLengthScale)   // Right
            };

            for (int i = 0; i < _driveRails.Length; i++)
            {
                _driveRails[i].transform.localScale = driveRailScaleOffsets[i];
            }
        }

        /// <summary>
        /// Updates the positions and scales of the bumpers, including their edges and corners.
        /// </summary>
        /// <remarks>This method adjusts both the edge and corner positions and scales of the bumpers
        /// to ensure they are correctly updated. It is intended to be called whenever the bumper layout or
        /// dimensions need to be refreshed.</remarks>
        private void UpdateBumpers()
        {
            UpdateBumperEdgePosition();
            UpdateBumperEdgeScale();

            UpdateBumperCornerPosition();
            UpdateBumperCornerScale();
        }

        /// <summary>
        /// Updates the positions and rotations of the bumper edges based on the robot's dimensions and perimeter
        /// multiplier.
        /// </summary>
        /// <remarks>This method recalculates the local positions and rotations of the bumper edges
        /// relative to the robot's center, using predefined offsets for each edge (front, back, left, and right). The
        /// offsets are determined by the robot's width, length, and a perimeter multiplier. The updated positions and
        /// rotations are applied to the corresponding transforms of the bumper edges.</remarks>
        private void UpdateBumperEdgePosition()
        {
            float xOffset = (_width / 2f) * _robotPerimeterMultiplier;
            float yOffset = 0f;
            float zOffset = (_length / 2f) * _robotPerimeterMultiplier;

            Quaternion[] bumperEdgeEulerOffsets =
            {
                Quaternion.Euler(0f, 0f,   0f), // Front
                Quaternion.Euler(0f, 180f, 0f), // Back
                Quaternion.Euler(0f, -90f, 0f), // Left
                Quaternion.Euler(0f, 90f,  0f)  // Right
            };

            Vector3[] bumperEdgePositionOffets = 
            {
                new (0f, yOffset,  zOffset),    // Front
                new (0f, yOffset, -zOffset),    // Back
                new (-xOffset, yOffset, 0f),    // Left
                new (xOffset,  yOffset, 0f),    // Right
            };

            for (int i = 0; i < _bumperEdges.Length; i++)
            {
                _bumperEdges[i].transform.SetLocalPositionAndRotation(bumperEdgePositionOffets[i], bumperEdgeEulerOffsets[i]);
            }
        }

        /// <summary>
        /// Updates the scale of the bumper edges based on the robot's dimensions and perimeter multiplier.
        /// </summary>
        /// <remarks>This method calculates the scale for each bumper edge using the robot's width,
        /// length, and a perimeter multiplier. The calculated scales are then applied to the local scale of the
        /// corresponding bumper edge transforms.</remarks>
        private void UpdateBumperEdgeScale()
        {
            float bumperEdgeWidthScale = _width * _robotPerimeterMultiplier;
            float bumperEdgeLengthScale = _length * _robotPerimeterMultiplier;

            Vector3[] bumperEdgeScaleOffsets =
            {
                new (bumperEdgeWidthScale,  1f, 1f),    // Front
                new (bumperEdgeWidthScale,  1f, 1f),    // Back
                new (bumperEdgeLengthScale, 1f, 1f),    // Left
                new (bumperEdgeLengthScale, 1f, 1f)     // Right
            };

            for (int i= 0; i < _bumperEdges.Length; i++)
            {
                _bumperEdges[i].transform.localScale = bumperEdgeScaleOffsets[i];
            }
        }

        /// <summary>
        /// Updates the positions and rotations of the bumper corners based on the robot's dimensions and perimeter
        /// multiplier.
        /// </summary>
        /// <remarks>This method calculates the offsets for each bumper corner using the robot's width,
        /// length, and perimeter multiplier. It then applies the calculated positions and rotations to the bumper
        /// corner transforms.</remarks>
        private void UpdateBumperCornerPosition()
        {
            float xOffset = (_width / 2f) * _robotPerimeterMultiplier;
            float yOffset = 0f;
            float zOffset = (_length / 2f) * _robotPerimeterMultiplier;

            Quaternion[] bumperCornerEulerOffsets =
            {
                Quaternion.Euler(0f,   0f, 0f), // Front Left
                Quaternion.Euler(0f,  90f, 0f), // Front Right
                Quaternion.Euler(0f, -90f, 0f), // Back Left
                Quaternion.Euler(0f, 180f, 0f)  // Back Right
            };

            Vector3[] bumperCornerPositionOffsets =
            {
                new (-xOffset, yOffset,  zOffset), // Front Left
                new ( xOffset, yOffset,  zOffset), // Front Right
                new (-xOffset, yOffset, -zOffset), // Back Left
                new ( xOffset, yOffset, -zOffset)  // Back Right
            };

            for (int i = 0; i < _bumperCorners.Length; i++)
            {
                _bumperCorners[i].transform.SetLocalPositionAndRotation(bumperCornerPositionOffsets[i], bumperCornerEulerOffsets[i]);
            }
        }

        /// <summary>
        /// Updates the scale of all bumper corner objects to the default scale.
        /// </summary>
        /// <remarks>This method iterates through all bumper corner objects and sets their local scale to
        /// a uniform scale of <see cref="Vector3.one"/>. Ensure that the <c>_bumperCorners</c> array is properly
        /// initialized and populated before calling this method.</remarks>
        private void UpdateBumperCornerScale()
        {
            Vector3 cornerScale = Vector3.one;

            for (int i = 0; i < _bumperCorners.Length; i++)
            {
                _bumperCorners[i].transform.localScale = cornerScale;
            }
        }

        /// <summary>
        /// Updates the position and scale of the belly pan.
        /// </summary>
        /// <remarks>This method adjusts the belly pan's position and scale to ensure it is correctly
        /// aligned and sized based on the current state of the system. It is intended to be called as part of the
        /// update process for maintaining the belly pan's configuration.</remarks>
        private void UpdateBellyPan()
        {
            UpdateBellyPanPosition();
            UpdateBellyPanScale();
        }

        /// <summary>
        /// Updates the position and rotation of the belly pan to align it with the current drive rail configuration.
        /// </summary>
        /// <remarks>This method adjusts the belly pan's local position and rotation based on the drive
        /// rail height, size multiplier, and belly pan thickness. The belly pan is repositioned relative to the drive
        /// rail to ensure proper alignment within the system.</remarks>
        private void UpdateBellyPanPosition()
        {
            float xOffset = 0f;
            float yOffset = -_driveRailHeight * _driveRailSizeMultiplier / 2f + _bellyPanThickness * _bellyPanThicknessMultiplier / 2f;
            float zOffset = 0f;
            Quaternion bellyPanEulerOffset = Quaternion.identity;

            _bellyPan.transform.SetLocalPositionAndRotation(new Vector3(xOffset, yOffset, zOffset), bellyPanEulerOffset);
        }

        /// <summary>
        /// Updates the scale of the belly pan based on the robot's dimensions and scaling multipliers.
        /// </summary>
        /// <remarks>This method calculates the belly pan's scale using the robot's width, length, and
        /// thickness, adjusted by specific multipliers. The calculated scale is then applied to the belly pan's
        /// transform. Ensure that all relevant dimensions and multipliers are set correctly before calling this
        /// method.</remarks>
        private void UpdateBellyPanScale()
        {
            float xScale = _width * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f;
            float yScale = _bellyPanThickness * _bellyPanThicknessMultiplier;
            float zScale = _length * _robotPerimeterMultiplier - _driveRailWidth * _driveRailSizeMultiplier * 2f;

            _bellyPan.transform.localScale = new Vector3(xScale, yScale, zScale);
        }
#endif
    }
}