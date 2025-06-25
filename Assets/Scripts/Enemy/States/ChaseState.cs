using UnityEngine;

namespace EnemyAI.States
{
    public class ChaseState : IAIState
    {
        public AIStateType StateType => AIStateType.Chase;

        private float lostPlayerTime;
        private const float MAX_CHASE_TIME_WITHOUT_SIGHT = 3f;
        private Vector3 lastKnownPosition;

        public void OnEnter(EnemyAIController controller)
        {
            lostPlayerTime = 0f;
            var blackboard = controller.Blackboard;
            lastKnownPosition = blackboard.lastKnownPlayerPosition;

            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsChasing", true);
            }
        }

        public void OnUpdate(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.canSeePlayer)
            {
                lostPlayerTime = 0f;
                lastKnownPosition = blackboard.player.position;
                controller.Movement.MoveTo(blackboard.player.position, true);
            }
            else
            {
                lostPlayerTime += Time.deltaTime;

                if (Vector3.Distance(controller.transform.position, lastKnownPosition) > 1f)
                {
                    controller.Movement.MoveTo(lastKnownPosition, true);
                }
                else
                {
                    controller.Movement.Stop();
                }
            }

            if (blackboard.player != null)
            {
                Vector3 directionToPlayer = (blackboard.player.position - controller.transform.position).normalized;
                directionToPlayer.y = 0;

                if (directionToPlayer.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    controller.transform.rotation = Quaternion.Slerp(
                        controller.transform.rotation,
                        targetRotation,
                        Time.deltaTime * 8f
                    );
                }
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsChasing", false);
            }
        }

        public AIStateType CheckTransitions(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.HealthPercentage < 0.2f)
            {
                return AIStateType.Flee;
            }

            if (blackboard.canSeePlayer && blackboard.distanceToPlayer <= controller.attackRange)
            {
                return AIStateType.Attack;
            }

            if (lostPlayerTime >= MAX_CHASE_TIME_WITHOUT_SIGHT)
            {
                return AIStateType.Search;
            }

            return AIStateType.Chase;
        }
    }
}