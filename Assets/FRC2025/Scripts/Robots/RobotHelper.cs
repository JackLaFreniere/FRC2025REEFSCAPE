using UnityEngine;

namespace FRC2025
{
    public class RobotHelper : MonoBehaviour
    {
        private static string _robotName;
        private const string _robotTag = "Robot";

        public static void SetRobotName(string name)
        {
            _robotName = name;
        }

        public static string GetRobotName()
        {
            return _robotName;
        }

        public static bool IsRobot(Collider collider, string tag = _robotTag)
        {
            return collider.CompareTag(tag);
        }

        public static bool IsRobot(GameObject gameObject, string tag = _robotTag)
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

        public static BaseRobot GetBaseRobotScript(string name)
        {
            return UnityEngine.GameObject.Find(name).GetComponent<BaseRobot>();
        }
    }
}