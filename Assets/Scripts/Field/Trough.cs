using System.Collections.Generic;
using UnityEngine;

public class Trough : ScoreableLocation
{
    [Header("Coral Scoring Settings")]
    [SerializeField] private CoralReefLocation coralReefLocation;

    private readonly Dictionary<int, int> coral = new ();

    /// <summary>
    /// Handles the trigger event when a child collider enters the trigger area.
    /// </summary>
    /// <remarks>If the collider is a valid scoring object, the method increments its score count.  If the
    /// collider is encountered for the first time, it is added to the scoring dictionary,  and the <see
    /// cref="OnScored"/> method is invoked.</remarks>
    /// <param name="collider">The <see cref="Collider"/> that entered the trigger area. Must represent a valid scoring object.</param>
    public void OnChildTriggerEnter(Collider collider)
    {
        if (!IsValidScoringObject(collider)) return;

        int id = collider.GetInstanceID();
        if (coral.ContainsKey(id))
        {
            coral[id]++;
        }
        else
        {
            coral[id] = 1;

            OnScored(collider);
        }
    }

    /// <summary>
    /// Handles the trigger stay event for child colliders and processes scoring logic.
    /// </summary>
    /// <remarks>This method checks if the provided collider is a valid scoring object. If it is, and it has
    /// not been processed before, it adds the collider to the scoring dictionary and invokes the scoring
    /// logic.</remarks>
    /// <param name="collider">The <see cref="Collider"/> involved in the trigger stay event. Must represent a valid scoring object.</param>
    public void OnChildTriggerStay(Collider collider)
    {
        if (!IsValidScoringObject(collider)) return;

        int id = collider.GetInstanceID();
        if (!coral.ContainsKey(id))
        {
            coral[id] = 1;

            OnScored(collider);
        }
    }

    /// <summary>
    /// Handles the event when a child object's trigger collider exits another collider.
    /// </summary>
    /// <remarks>This method checks if the exiting collider is a valid scoring object. If it is, the method
    /// decrements its associated score count. When the score count reaches zero, the object is removed from the
    /// tracking collection, and the <see cref="OnUnscored"/> method is invoked.</remarks>
    /// <param name="collider">The collider that exited the trigger area.</param>
    public void OnChildTriggerExit(Collider collider)
    {
        if (!IsValidScoringObject(collider)) return;

        int id = collider.GetInstanceID();
        if (coral.ContainsKey(id))
        {
            coral[id]--;
            if (coral[id] <= 0)
            {
                coral.Remove(id);

                OnUnscored(collider);
            }
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="Collider"/> is a valid scoring object.
    /// </summary>
    /// <param name="other">The <see cref="Collider"/> to evaluate.</param>
    /// <returns><see langword="true"/> if the <paramref name="other"/> collider has the specified scoring tag  and is the root
    /// of its transform hierarchy; otherwise, <see langword="false"/>.</returns>
    public override bool IsValidScoringObject(Collider other)
    {
        return other.CompareTag(scoringElementTag) && other.transform.root == other.transform;
    }

    /// <summary>
    /// Handles the scoring event when a collision occurs with the specified object.
    /// </summary>
    /// <remarks>This method updates the score by calculating the points associated with the specified
    /// collider and applying them to the score manager for the current alliance.</remarks>
    /// <param name="other">The <see cref="Collider"/> involved in the scoring event. This represents the object that triggered the score.</param>
    public override void OnScored(Collider other)
    {
        scoreManager.AddScore(GetScore(other), allianceColor);
    }

    /// <summary>
    /// Handles the event when an object is unscored, deducting points from the score.
    /// </summary>
    /// <remarks>This method deducts points from the score based on the object represented by <paramref
    /// name="other"/>. The deduction amount is determined by the <c>GetUnscore</c> method, and the score is updated for
    /// the specified alliance.</remarks>
    /// <param name="other">The <see cref="Collider"/> representing the object that triggered the unscore event.</param>
    public void OnUnscored(Collider other)
    {
        scoreManager.AddScore(-GetUnscore(other), allianceColor);
    }

    /// <summary>
    /// Calculates the total score for a given collider, including any applicable bonus points.
    /// </summary>
    /// <param name="other">The collider representing the object for which the score is being calculated. Must not be null.</param>
    /// <returns>The total score, which includes the base score and any bonus points if applicable.</returns>
    private int GetScore(Collider other)
    {
        int bonusScore = 0;
        if (Timer.IsAuto())
        {
            bonusScore = coralReefLocation.autoBonus;
            other.GetComponent<Coral>().SetScoredInAuto(true);
        }

        return coralReefLocation.score + bonusScore;
    }

    /// <summary>
    /// Calculates the total score for a coral, including any applicable bonus points.
    /// </summary>
    /// <param name="other">The collider associated with the coral to evaluate. Must contain a <see cref="Coral"/> component.</param>
    /// <returns>The total score for the coral, including the base score and any bonus points if applicable.</returns>
    private int GetUnscore(Collider other)
    {
        int bonusScore = 0;
        if (other.GetComponent<Coral>().GetScoredInAuto())
        {
            bonusScore = coralReefLocation.autoBonus;
            other.GetComponent<Coral>().SetScoredInAuto(false);
        }

        return coralReefLocation.score + bonusScore;
    }
}