using UnityEngine;

namespace FRC2025
{
    public static class RobotHelper
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
            
            return baseRobot.AllianceColor == allianceColor;
        }

        public static bool IsRobotOnAlliance(GameObject gameObject, AllianceColor allianceColor)
        {
            BaseRobot baseRobot = GetBaseRobotScript(gameObject);
            
            return baseRobot.AllianceColor == allianceColor;
        }

        public static float UnitToMeters(UnitType unit) => unit switch
        {
            UnitType.Meters => 1f,
            UnitType.Centimeters => 0.01f,
            UnitType.Inches => 0.0254f,
            _ => 1f
        };

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
#pragma warning disable UNT0026, IDE0270
            var comp = go.GetComponent<T>();
            if (comp == null)
                comp = go.AddComponent<T>();
            return comp;
#pragma warning restore UNT0026, IDE0270
        }
    }
}