using UnityEngine;
namespace EnemyAI.States
{
    public class PatrolState : IAIState
    {
        public AIStateType StateType => AIStateType.Patrol;

        private float waitTimer;
        private bool isWaiting;

        public void OnEnter(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.patrolPoints == null || blackboard.patrolPoints.Length == 0)
                return;

            MoveToNextPatrolPoint(controller);
            isWaiting = false;
            waitTimer = 0f;
        }

        public void OnUpdate(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.patrolPoints == null || blackboard.patrolPoints.Length == 0)
                return;

            if (isWaiting)
            {
                waitTimer += Time.deltaTime;

                PerformCasualLookAround(controller);

                if (waitTimer >= controller.patrolWaitTime)
                {
                    MoveToNextPatrolPoint(controller);
                    isWaiting = false;
                    waitTimer = 0f;
                }
            }
            else
            {
                if (blackboard.hasReachedDestination)
                {
                    isWaiting = true;
                    controller.Movement.Stop();
                }
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            isWaiting = false;
            waitTimer = 0f;
        }

        public AIStateType CheckTransitions(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.canSeePlayer)
            {
                return AIStateType.Chase;
            }

            if (blackboard.isAlerted)
            {
                return AIStateType.Search;
            }

            if (blackboard.patrolPoints == null || blackboard.patrolPoints.Length == 0)
            {
                return AIStateType.Idle;
            }
            return AIStateType.Patrol;
        }
        private void MoveToNextPatrolPoint(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            blackboard.currentPatrolIndex = (blackboard.currentPatrolIndex + 1) % blackboard.patrolPoints.Length;
            Vector3 destination = blackboard.patrolPoints[blackboard.currentPatrolIndex];

            controller.Movement.MoveTo(destination, false);
        }
        private void PerformCasualLookAround(EnemyAIController controller)
        {
            if (waitTimer > controller.patrolWaitTime * 0.3f && waitTimer < controller.patrolWaitTime * 0.7f)
            {
                float lookAngle = Mathf.Sin(Time.time * 0.5f) * 45f;
                Vector3 lookDirection = Quaternion.AngleAxis(lookAngle, Vector3.up) * controller.transform.forward;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

                controller.transform.rotation = Quaternion.Slerp(
                    controller.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 1f
                );
            }
        }

    }
}