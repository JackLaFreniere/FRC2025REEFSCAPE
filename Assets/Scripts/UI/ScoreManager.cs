using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private string blueButtonName;
    [SerializeField] private string redButtonName;

    private Button blueButton;
    private Button redButton;

    private int blueScore = 0;
    private int redScore = 0;

    private void Start()
    {
        blueButton = GetButton(blueButtonName);
        redButton = GetButton(redButtonName);

        UpdateScore();
    }

    /// <summary>
    /// Updates the displayed scores for the blue and red alliances.
    /// </summary>
    /// <remarks>This method updates the text of the blue and red score buttons to reflect the current values
    /// of the <c>blueScore</c> and <c>redScore</c> fields, respectively. Ensure that these fields are properly
    /// initialized and updated before calling this method.</remarks>
    private void UpdateScore()
    {
        blueButton.text = blueScore.ToString();
        redButton.text = redScore.ToString();
    }

    /// <summary>
    /// Adds the specified number of points to the score of the given alliance.
    /// </summary>
    /// <remarks>This method updates the score for the specified alliance and triggers a score update
    /// operation.</remarks>
    /// <param name="points">The number of points to add. Must be a non-negative integer.</param>
    /// <param name="allianceColor">The alliance to which the points should be added. Valid values are <see cref="AllianceColor.Blue"/> and <see
    /// cref="AllianceColor.Red"/>.</param>
    public void AddScore(int points, AllianceColor allianceColor)
    {
        switch (allianceColor)
        {
            case AllianceColor.Blue:
                blueScore += points;
                break;
            case AllianceColor.Red:
                redScore += points;
                break;
        }

        UpdateScore();
    }

    /// <summary>
    /// Retrieves a button from the UI document by its name.
    /// </summary>
    /// <param name="name">The name of the button to retrieve. This value cannot be null or empty.</param>
    /// <returns>The <see cref="Button"/> with the specified name, or <see langword="null"/> if no button with the given name is
    /// found.</returns>
    private Button GetButton(string name)
    {
        return GetComponent<UIDocument>().rootVisualElement.Q<Button>(name);
    }
}
