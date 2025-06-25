using UnityEngine;

namespace EnemyAI.States
{
    public class FleeState : IAIState
    {
        public AIStateType StateType => AIStateType.Flee;

        private Vector3 fleeDestination;
        private bool hasFleeDestination;
        private float fleeTime;
        private const float MAX_FLEE_TIME = 8f;

        public void OnEnter(EnemyAIController controller)
        {
            fleeTime = 0f;
            hasFleeDestination = false;
            FindFleeDestination(controller);
            
            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsFleeing", true);
            }
        }

        public void OnUpdate(EnemyAIController controller)
        {
            fleeTime += Time.deltaTime;
            var blackboard = controller.Blackboard;

            if (!hasFleeDestination)
            {
                FindFleeDestination(controller);
                return;
            }

            // 도망칠 위치로 이동
            controller.Movement.MoveTo(fleeDestination, true); // 뛰어서 도망

            // 도망치면서도 가끔 플레이어 확인
            if (blackboard.player != null && fleeTime % 2f < 0.1f)
            {
                Vector3 directionToPlayer = (blackboard.player.position - controller.transform.position).normalized;
                Vector3 fleeDirection = -directionToPlayer; // 플레이어 반대 방향
                
                // 새로운 도망 지점 업데이트
                Vector3 newFleePoint;
                if (controller.Movement.FindRandomPoint(
                    controller.transform.position + fleeDirection * 10f, 5f, out newFleePoint))
                {
                    fleeDestination = newFleePoint;
                }
            }

            // 목적지에 도착했으면 새로운 지점 찾기
            if (blackboard.hasReachedDestination)
            {
                hasFleeDestination = false;
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsFleeing", false);
            }
        }

        public AIStateType CheckTransitions(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            // 체력이 회복되고 플레이어가 멀리 있으면 다른 상태로
            if (blackboard.HealthPercentage > 0.4f && blackboard.distanceToPlayer > 15f)
            {
                if (blackboard.canSeePlayer)
                {
                    return AIStateType.Chase;
                }
                else
                {
                    return AIStateType.Search;
                }
            }

            // 너무 오래 도망쳤으면 수색 상태로
            if (fleeTime >= MAX_FLEE_TIME)
            {
                return AIStateType.Search;
            }

            return AIStateType.Flee;
        }

        private void FindFleeDestination(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;
            
            Vector3 fleeDirection;
            
            if (blackboard.player != null)
            {
                // 플레이어 반대 방향으로 도망
                fleeDirection = (controller.transform.position - blackboard.player.position).normalized;
            }
            else
            {
                // 플레이어가 없으면 랜덤 방향
                fleeDirection = Random.insideUnitSphere.normalized;
                fleeDirection.y = 0;
            }

            Vector3 fleePoint = controller.transform.position + fleeDirection * 15f;
            
            Vector3 validFleePoint;
            if (controller.Movement.FindRandomPoint(fleePoint, 5f, out validFleePoint))
            {
                fleeDestination = validFleePoint;
                hasFleeDestination = true;
            }
            else
            {
                // 적절한 지점을 못 찾으면 현재 위치에서 멀리
                fleeDestination = controller.transform.position + fleeDirection * 5f;
                hasFleeDestination = true;
            }
        }
    }
}
