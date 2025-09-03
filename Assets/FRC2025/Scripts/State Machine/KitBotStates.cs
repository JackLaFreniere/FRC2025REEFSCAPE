using UnityEngine;

namespace FRC2025
{
    public class KitBotIdleState : PlayerState
    {
        public KitBotIdleState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }
    }

    public class KitBotCoralScoreState : PlayerState
    {
        private GameObject scoringMechanism;
        private readonly float rotationSpeed = 360f;
        private Vector3 rotationAxis = Vector3.left;
        private GameObject[] scoringWheels;


        public KitBotCoralScoreState(BaseRobot player, StateMachine stateMachine) : base(player, stateMachine) { }

        public override void Enter()
        {
            scoringMechanism = BaseRobot.gameObject.transform.Find("Coral_Scoring_Mechanism").gameObject;

            scoringWheels = GameObject.FindGameObjectsWithTag("CoralScoringWheel");

            foreach (GameObject wheel in scoringWheels)
            {
                wheel.GetComponent<CapsuleCollider>().enabled = false;
            }
        }

        public override void Update()
        {
            scoringMechanism.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        public override void Exit()
        {
            foreach (GameObject wheel in scoringWheels)
            {
                wheel.GetComponent<CapsuleCollider>().enabled = true;
            }
        }
    }
}