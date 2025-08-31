using UnityEngine;

namespace FRC2025
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Game Info Scriptable Object")]
        [SerializeField] private GameInfo gameInfo;

        private static GameInfo _instance;

        private void Awake()
        {
            _instance = gameInfo;
        }

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