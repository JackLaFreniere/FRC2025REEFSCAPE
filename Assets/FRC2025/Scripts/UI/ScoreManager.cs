using UnityEngine;

namespace FRC2025
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Game Info Scriptable Object")]
        [SerializeField] private GameInfo gameInfo;

        private static GameInfo _instance;

        /// <summary>
        /// Initializes the singleton instance of the game information.
        /// </summary>
        /// <remarks>This method is called automatically by Unity when the script instance is being
        /// loaded. It assigns the singleton instance to the specified game information object.</remarks>
        private void Awake()
        {
            _instance = gameInfo;
        }

        /// <summary>
        /// Adds the specified number of points to the score of the given alliance.
        /// </summary>
        /// <remarks>Points are only added if the match is not over. If the match has ended, this method
        /// has no effect.</remarks>
        /// <param name="points">The number of points to add. Must be a non-negative integer.</param>
        /// <param name="allianceColor">The alliance to which the points will be added.  Use <see cref="AllianceColor.Blue"/> for the blue alliance
        /// or <see cref="AllianceColor.Red"/> for the red alliance.</param>
        public static void AddScore(int points, AllianceColor allianceColor)
        {
            if (Timer.IsMatchOver()) return;

            switch (allianceColor)
            {
                case AllianceColor.Blue:
                    _instance.BlueScore += points;
                    break;
                case AllianceColor.Red:
                    _instance.RedScore += points;
                    break;
            }
        }
    }
}