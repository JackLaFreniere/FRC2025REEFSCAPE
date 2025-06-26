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
}
