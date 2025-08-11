using UnityEngine;

[CreateAssetMenu(fileName = "CoralReefLocation", menuName = "Scriptable Objects/CoralReefLocation")]
public class CoralReefLocation : ScriptableObject
{
    public CoralReefLevel level;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public int score;
}