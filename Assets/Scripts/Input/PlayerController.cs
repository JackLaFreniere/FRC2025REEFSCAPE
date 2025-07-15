using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public RobotInfo robotInfo;
    private IRobotInputHandler robotActions;
    private GameObject robotInstance;

    private void Awake()
    {
        robotInstance = Instantiate(robotInfo.robotPrefab, robotInfo.spawnPosition, Quaternion.Euler(robotInfo.spawnEuler));
        
        BaseRobot baseRobotScript = robotInstance.GetComponent<BaseRobot>();
        baseRobotScript.SetRobotInfo(robotInfo);

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