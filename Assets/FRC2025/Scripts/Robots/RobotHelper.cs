using UnityEngine;

namespace FRC2025
{
    public class RobotHelper : MonoBehaviour
    {
        private const string RobotTag = "Robot";

        public static bool IsRobot(Collider collider, string tag = RobotTag)
        {
            return collider.CompareTag(tag);
        }

        public static bool IsRobot(GameObject gameObject, string tag = RobotTag)
        {
            return gameObject.CompareTag(tag);
        }

        public static BaseRobot GetBaseRobotScript(Collider collider)
        {
            return collider.GetComponentInParent<BaseRobot>();
        }

        public static BaseRobot GetBaseRobotScript(GameObject gameObject)
        {
            return gameObject.GetComponentInParent<BaseRobot>();
        }
    }
}