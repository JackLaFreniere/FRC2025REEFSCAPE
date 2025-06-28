using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public RobotInfo robotInfo;
    private IRobotInputHandler robotActions;
    private GameObject robotInstance;

    private void Awake()
    {
        robotInstance = Instantiate(robotInfo.robotPrefab, new Vector3(4f, 0.1f, -2f), Quaternion.Euler(robotInfo.spawnEuler));

        robotActions = robotInstance.GetComponent<IRobotInputHandler>();
        this.GetComponent<PlayerInput>().actions = robotInfo.playerInput;

        robotActions.SetBaseRobot(robotInstance.GetComponent<BaseRobot>());
        robotActions.InputAwake();
    }

    private void OnEnable()
    {
        robotActions.InputOnEnable();
    }

    private void OnDisable()
    {
        robotActions.InputOnDisable();
    }
}