using UnityEngine;

namespace EnemyAI.States
{
    public class AttackState : IAIState
    {
        public AIStateType StateType => AIStateType.Attack;

        private float attackTimer;
        private bool hasAttacked;

        public void OnEnter(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;
            attackTimer = 0f;
            hasAttacked = false;

            controller.Movement.Stop();

            if (blackboard.player != null)
            {
                Vector3 direction = (blackboard.player.position - controller.transform.position).normalized;
                direction.y = 0;
                controller.transform.rotation = Quaternion.LookRotation(direction);
            }

            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("IsAttacking", true);
            }
        }

        public void OnUpdate(EnemyAIController controller)
        {
            attackTimer += Time.deltaTime;
            var blackboard = controller.Blackboard;

            if (blackboard.player != null)
            {
                Vector3 direction = (blackboard.player.position - controller.transform.position).normalized;
                direction.y = 0;

                if (direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    controller.transform.rotation = Quaternion.Slerp(
                        controller.transform.rotation,
                        targetRotation,
                        Time.deltaTime * 10f
                    );
                }
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;
            blackboard.lastAttackTime = Time.time;
            blackboard.isAttacking = false;

            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsAttacking", false);
            }
        }

        public AIStateType CheckTransitions(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.HealthPercentage < 0.2f)
            {
                return AIStateType.Flee;
            }

            if (attackTimer >= 1.5f)
            {
                if (blackboard.canSeePlayer)
                {
                    if (blackboard.distanceToPlayer <= controller.attackRange &&
                        Time.time - blackboard.lastAttackTime < blackboard.attackCooldown)
                    {
                        return AIStateType.Chase;
                    }
                    else if (blackboard.distanceToPlayer <= controller.attackRange)
                    {
                        return AIStateType.Attack;
                    }
                    else
                    {
                        return AIStateType.Chase;
                    }
                }
                else
                {
                    return AIStateType.Search;
                }
            }
            return AIStateType.Attack;
        }

        private void PerformAttack(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            if (blackboard.player != null && blackboard.distanceToPlayer <= controller.attackRange)
            {
                controller.AttackPlayer();

                // Sound, Effact
            }
        }
    }
}