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
#pragma warning restore CS0414

        private void Awake()
        {
            _name = "Cascading Elevator";
        }

        private void Update()
        {
            UpdateElevatorMultipliers();

            // Validate main parents
            ValidateDirectory(ref _baseStageParent, _baseStageParentName);
            ValidateDirectory(ref _topStageParent, _topStageParentName);
            ValidateDirectory(ref _intermediateStagesParent, _intermediateStagesParentName);

            // Validate base and top stages (each has 2 cubes)
            _baseStage = ValidateStage(_baseStage, _baseStageParent, _baseStageParentName);
            _topStage = ValidateStage(_topStage, _topStageParent, _topStageParentName);

            // Handle intermediate stage subparents and cubes
            ValidateIntermediateStages();

            UpdateElevatorOffset();

            ConfigureStage(1, _baseStage);
            ConfigureStage(_numStages, _topStage);

            for (int i = 2; i < _numStages; i++)
            {
                ConfigureStage(i, _intermediateStages[i - 2]);
            }
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
#endif
    }
}