using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "RobotInfo", menuName = "Scriptable Objects/RobotInfo")]
public class RobotInfo : ScriptableObject
{
    public GameObject robotPrefab;
    public InputActionAsset playerInput;
    public IRobotInputHandler robotInputHandler;
    public Vector3 spawnEuler;
    public Vector3 spawnPosition = new (4f, 0.1f, -2f);
}
