public interface IRobotInputHandler
{
    void SetBaseRobot(BaseRobot robot);
    void InputAwake();
    void InputOnEnable();
    void InputOnDisable();
}