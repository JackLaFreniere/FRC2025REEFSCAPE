using UnityEngine;

[CreateAssetMenu(fileName = "CoralReefLocation", menuName = "Scriptable Objects/CoralReefLocation")]
public class CoralReefLocation : ScriptableObject
{
    public ReefLevel level;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
}