using UnityEngine;

namespace FRC2025
{
    public class CoralStation : HumanPlayerStation
    {
        private BaseRobot _baseRobot;

        /// <summary>
        /// Handles the event when another collider enters the trigger collider attached to this object.
        /// </summary>
        /// <remarks>This method checks if the entering object is a robot and whether it is carrying coral. If the
        /// conditions are met, it triggers the logic to drop a scoring element.</remarks>
        /// <param name="other">The <see cref="Collider"/> of the object that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!RobotHelper.IsRobot(other)) return;

            if (_baseRobot == null || !_baseRobot.gameObject.Equals(other.gameObject))
            {
                RobotHelper.CacheBaseRobot(other, ref _baseRobot);
            }

            if (_baseRobot.hasCoral) return;

            DropScoringElement();
        }

        /// <summary>
        /// Spawns a scoring element at a specified position and rotation.
        /// </summary>
        /// <remarks>This method instantiates a scoring element using the predefined prefab, position, and
        /// rotation. Ensure that the prefab is assigned and valid before calling this method.</remarks>
        public override void DropScoringElement()
        {
            Instantiate(_scoringElementPrefab, GetScoringElementPosition(), GetScoringElementRotation());
        }
    }
}