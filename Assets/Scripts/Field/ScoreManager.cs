using JetBrains.Annotations;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static int score = 0;

    private void Start()
    {
        // Resets the score at the start of the game
        score = 0;
        Debug.Log(score);
    }

    /// <summary>
    /// ncrements the score by the given amount of points.
    /// </summary>
    /// <param name="points">The amount of points being added</param>
    public static void AddScore(int points)
    {
        score += points;
        Debug.Log(score);
    }
}
