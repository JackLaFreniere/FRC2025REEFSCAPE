using JetBrains.Annotations;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static int score = 0;

    private void Start()
    {
        Debug.Log(score);
    }

    public static void AddScore(int points)
    {
        score += points;
        Debug.Log(score);
    }
}
