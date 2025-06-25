using UnityEngine;

namespace EnemyAI.States
{
    public class SearchState : IAIState
    {
        public AIStateType StateType => AIStateType.Search;

        private float searchTime;
        private const float MAX_SEARCH_TIME = 10f;
        private Vector3 searchCenter;
        private Vector3 currentSearchPoint;
        private bool hasSearchPoint;
        private int searchAttempts;
        private const int MAX_SEARCH_ATTEMPTS = 5;

        public void OnEnter(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;
            searchTime = 0f;
            searchAttempts = 0;
            
            // 마지막으로 플레이어를 본 위치를 중심으로 수색
            searchCenter = blackboard.lastKnownPlayerPosition;
            hasSearchPoint = false;
            
            FindNextSearchPoint(controller);
            
            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsSearching", true);
            }
        }

        public void OnUpdate(EnemyAIController controller)
        {
            searchTime += Time.deltaTime;

            if (!hasSearchPoint)
            {
                FindNextSearchPoint(controller);
                return;
            }

            // 수색 지점으로 이동
            if (Vector3.Distance(controller.transform.position, currentSearchPoint) > 1.5f)
            {
                controller.Movement.MoveTo(currentSearchPoint, false);
            }
            else
            {
                // 수색 지점에 도착했으면 주변을 살펴봄
                SearchAround(controller);
                
                // 잠시 후 다음 수색 지점으로
                if (searchTime > 2f)
                {
                    hasSearchPoint = false;
                    searchAttempts++;
                }
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsSearching", false);
            }
        }

        public AIStateType CheckTransitions(EnemyAIController controller)
        {
            var blackboard = controller.Blackboard;

            // 플레이어 재발견
            if (blackboard.canSeePlayer)
            {
                return AIStateType.Chase;
            }

            // 수색 시간 초과 또는 시도 횟수 초과
            if (searchTime >= MAX_SEARCH_TIME || searchAttempts >= MAX_SEARCH_ATTEMPTS)
            {
                // 경계 레벨에 따라 다른 상태로 전환
                if (blackboard.alertLevel > 0.5f)
                {
                    return AIStateType.Patrol; // 순찰 재개
                }
                else
                {
                    return AIStateType.Idle; // 대기 상태로
                }
            }

            return AIStateType.Search;
        }

        private void FindNextSearchPoint(EnemyAIController controller)
        {
            // 마지막 플레이어 위치 주변에서 랜덤 지점 찾기
            Vector3 randomPoint;
            if (controller.Movement.FindRandomPoint(searchCenter, 8f, out randomPoint))
            {
                currentSearchPoint = randomPoint;
                hasSearchPoint = true;
                searchTime = 0f;
            }
            else
            {
                // 적절한 지점을 못 찾으면 제자리에서 수색
                currentSearchPoint = controller.transform.position;
                hasSearchPoint = true;
            }
        }

        private void SearchAround(EnemyAIController controller)
        {
            // 360도 회전하며 수색
            float rotationSpeed = 90f; // 90도/초
            controller.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}