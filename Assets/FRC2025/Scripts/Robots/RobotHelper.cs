using UnityEngine;

namespace FRC2025
{
    public class RobotHelper : MonoBehaviour
    {
        private const string _robotTag = "Robot";

        public static bool IsRobot(Collider collider)
        {
            return collider.CompareTag(_robotTag);
        }

        public static bool IsRobot(GameObject gameObject)
        {
            return gameObject.CompareTag(_robotTag);
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
            return GameObject.Find(name).GetComponent<BaseRobot>();
        }

        public static void CacheBaseRobot(Collider collider, ref BaseRobot baseRobot)
        {
            if (baseRobot == null)
            {
                baseRobot = GetBaseRobotScript(collider);
            }
        }

        public static void CacheBaseRobot(GameObject gameObject, ref BaseRobot baseRobot)
        {
            if (baseRobot == null)
            {
                baseRobot = GetBaseRobotScript(gameObject);
            }
        }

        public static bool IsRobotOnAlliance(Collider other, AllianceColor allianceColor)
        {
            BaseRobot baseRobot = GetBaseRobotScript(other);
            
            return baseRobot.allianceColor == allianceColor;
        }

        public static bool IsRobotOnAlliance(GameObject gameObject, AllianceColor allianceColor)
        {
            BaseRobot baseRobot = GetBaseRobotScript(gameObject);
            
            return baseRobot.allianceColor == allianceColor;
        }
    }
}