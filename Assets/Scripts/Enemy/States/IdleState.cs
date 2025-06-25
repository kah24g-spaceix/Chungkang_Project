using UnityEngine;
namespace EnemyAI.States
{
    public class IdleState : IAIState
    {
        public AIStateType StateType => AIStateType.Idle;

        private float idleTime;
        private float maxIdleTime = 3f;

        public void OnEnter(EnemyAIController controller)
        {
            controller.Movement.Stop();
            idleTime = 0f;
            maxIdleTime = Random.Range(2f, 5f);

            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetFloat("Speed", 0f);
            }
        }

        public void OnUpdate(EnemyAIController controller)
        {
            idleTime += Time.deltaTime;

            if (idleTime > maxIdleTime * 0.6f)
            {
                LookAround(controller);
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            idleTime = 0f;
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

            if (idleTime >= maxIdleTime)
            {
                return blackboard.patrolPoints?.Length > 0 ? AIStateType.Patrol : AIStateType.Idle;
            }

            return AIStateType.Idle;
        }

        private void LookAround(EnemyAIController controller)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;
            randomDirection.Normalize();

            Vector3 lookTarget = controller.transform.position + randomDirection * 2f;
            Vector3 direction = (lookTarget - controller.transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation,
                targetRotation,
                Time.deltaTime * 2f
            );
        }
    }
}