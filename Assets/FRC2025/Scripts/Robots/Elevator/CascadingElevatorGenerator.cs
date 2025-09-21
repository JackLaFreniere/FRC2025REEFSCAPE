using UnityEngine;

namespace FRC2025
{
    [ExecuteInEditMode]
    public class CascadingElevatorGenerator : Generator<CascadingElevatorSubsystem>
    {
        [Header("Cascading Elevator Settings")]
        [SerializeField] private UnitType _tubingUnit = UnitType.Inches;
        [SerializeField, Min(0)] private float _tubingLength = 2f;
        [SerializeField, Min(0f)] private float _tubingWidth = 1f;
        [SerializeField, Min(0f)] private float _tubingHeight = 12f;

        private void Awake()
        {
            _name = "Cascading Elevator";
        }
    }
}