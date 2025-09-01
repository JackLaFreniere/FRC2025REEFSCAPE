using System;
using System.Collections;
using UnityEngine;

namespace FRC2025
{
    public class Timer : MonoBehaviour
    {
        [Header("Game Info Scriptable Object")]
        [SerializeField] private GameInfo gameInfo;

        private readonly static int startTime = 150;
        private readonly static int autoDuration = 15;
        private readonly static int endgameDuration = 20;

        public static event Action MatchStart;
        public static event Action AutoEnd;
        public static event Action EndgameStart;
        public static event Action MatchOver;

        private static int timeLeft;

        /// <summary>
        /// Initializes the game state when the object is enabled.
        /// </summary>
        /// <remarks>Resets the game timer and scores for both teams to their initial values.  This method
        /// is called automatically when the object becomes active.</remarks>
        private void OnEnable()
        {
            gameInfo.TimeLeft = FormatTime(startTime);
            gameInfo.BlueScore = 0;
            gameInfo.RedScore = 0;

            timeLeft = FormatTime(gameInfo.TimeLeft);
        }

        /// <summary>
        /// Initiates the game start sequence, including updating game information, starting the timer countdown, and
        /// raising the match start event.
        /// </summary>
        /// <remarks>This method performs the necessary setup for the start of a match. It updates the
        /// game information time, begins the countdown timer,  and invokes the <see cref="MatchStart"/> event to notify
        /// subscribers that the match has started.</remarks>
        private void Start()
        {
            UpdateGameInfoTime();
            StartCoroutine(TimerCountdown());
            MatchStart?.Invoke();
        }

        /// <summary>
        /// Manages the countdown timer for the match, invoking events at specific milestones.
        /// </summary>
        /// <remarks>This method decrements the timer every second and triggers specific events based on
        /// the remaining time: <list type="bullet"> <item><description><see cref="AutoEnd"/> is invoked when the time
        /// left reaches <c>startTime - autoDuration - 1</c>.</description></item> <item><description><see
        /// cref="EndgameStart"/> is invoked when the time left equals <c>endgameDuration</c>.</description></item>
        /// <item><description><see cref="MatchOver"/> is invoked when the timer reaches zero.</description></item>
        /// </list> The method updates the game state by calling <see cref="UpdateGameInfoTime"/> after each
        /// decrement.</remarks>
        /// <returns>An enumerator that can be used to control the coroutine execution.</returns>
        private IEnumerator TimerCountdown()
        {
            while (timeLeft > 0)
            {
                yield return new WaitForSeconds(1f);
                timeLeft--;
                UpdateGameInfoTime();

                if (timeLeft == startTime - autoDuration - 1)
                    AutoEnd?.Invoke();
                else if (timeLeft == endgameDuration)
                    EndgameStart?.Invoke();
            }

            MatchOver?.Invoke();
        }

        /// <summary>
        /// Updates the remaining time in the game information.
        /// </summary>
        /// <remarks>This method updates the <c>TimeLeft</c> property of the <c>gameInfo</c> object  with
        /// the formatted value of the current remaining time. If <c>gameInfo</c> is  <see langword="null"/>, the method
        /// does nothing.</remarks>
        private void UpdateGameInfoTime()
        {
            if (gameInfo != null)
            {
                gameInfo.TimeLeft = FormatTime(timeLeft);
            }
        }

        /// <summary>
        /// Formats a time duration, given in seconds, into a string representation in the format "M:SS".
        /// </summary>
        /// <param name="seconds">The total time duration, in seconds. Must be a non-negative integer.</param>
        /// <returns>A string representing the time duration in the format "M:SS", where "M" is the number of minutes and "SS" is
        /// the number of seconds.</returns>
        private string FormatTime(int seconds)
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;
            return $"{minutes:D1}:{remainingSeconds:D2}";
        }

        /// <summary>
        /// Converts a time string in the format "MM:SS" to the total number of seconds.
        /// </summary>
        /// <param name="seconds">A string representing the time in the format "MM:SS".</param>
        /// <returns>The total number of seconds as an integer.</returns>
        private int FormatTime(string seconds)
        {
            return int.Parse(seconds.Split(':')[0]) * 60 + int.Parse(seconds.Split(':')[1]);
        }

        /// <summary>
        /// Determines whether the current state is within the autonomous period.
        /// </summary>
        /// <returns><see langword="true"/> if the current time is within the autonomous period; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsAuto()
        {
            return timeLeft >= startTime - autoDuration;
        }

        /// <summary>
        /// Determines whether the game is in the endgame phase based on the remaining time.
        /// </summary>
        /// <returns><see langword="true"/> if the remaining time is less than or equal to the endgame duration; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsEndgame()
        {
            return timeLeft <= endgameDuration;
        }

        /// <summary>
        /// Determines whether the match has ended.
        /// </summary>
        /// <returns><see langword="true"/> if the match is over; otherwise, <see langword="false"/>.</returns>
        public static bool IsMatchOver()
        {
            return timeLeft == 0;
        }
    }
}