using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    private Button timeButton;
    [SerializeField] private string buttonName;
    private int timeLeft = 150;

    private void Start()
    {
        timeButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>(buttonName);
        UpdateButtonValue();

        StartCoroutine(TimerCountdown());
    }

    /// <summary>
    /// Executes a countdown timer, decrementing the remaining time at one-second intervals.
    /// </summary>
    /// <remarks>This method is a coroutine that waits for one second between each decrement of the 
    /// <c>timeLeft</c> variable. It updates the button value after each decrement. The countdown  stops when
    /// <c>timeLeft</c> reaches zero.</remarks>
    /// <returns></returns>
    private IEnumerator TimerCountdown()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
            UpdateButtonValue();
        }
    }

    /// <summary>
    /// Updates the text of the time button to reflect the current time left.
    /// </summary>
    /// <remarks>This method formats the remaining time and sets it as the text of the time button. Ensure
    /// that <c>timeLeft</c> contains a valid value before calling this method.</remarks>
    private void UpdateButtonValue()
    {
        timeButton.text = FormatTime(timeLeft);
    }

    /// <summary>
    /// Formats a time duration, given in seconds, into a string representation in the format "M:SS".
    /// </summary>
    /// <param name="seconds">The total time duration, in seconds. Must be a non-negative integer.</param>
    /// <returns>A string representing the time duration in the format "M:SS", where M is the number of minutes and SS is the
    /// number of seconds, zero-padded to two digits.</returns>
    private string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:D1}:{remainingSeconds:D2}";
    }
}
