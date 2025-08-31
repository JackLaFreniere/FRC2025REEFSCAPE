using UnityEngine;

public class ScoringElement : MonoBehaviour
{
    protected bool _isScored = false;
    protected bool _scoredInAuto = false;

    public bool GetIsScored() { return _isScored; }
    public bool GetScoredInAuto() { return _scoredInAuto; }
    public void SetScored(bool value = true) { _isScored = value; }
    public void SetScoredInAuto(bool value = true) { _scoredInAuto = value; }
}
