using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private RobotInfo robotInfo;
    private IRobotInputHandler robotActions;
    private GameObject robotInstance;

    private void Awake()
    {
        robotInstance = Instantiate(robotInfo.robotPrefab, new Vector3(4f, 0.1f, -2f), Quaternion.Euler(90f, 90f, 0f));

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