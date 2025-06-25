using UnityEngine;

namespace EnemyAI.States
{
    public class DeadState : IAIState
    {
        public AIStateType StateType => AIStateType.Dead;

        private bool hasPlayedDeathAnimation;
        private float deathTime;

        public void OnEnter(EnemyAIController controller)
        {
            hasPlayedDeathAnimation = false;
            deathTime = 0f;
            
            // 모든 움직임 정지
            controller.Movement.Stop();
            controller.Movement.SetMovementEnabled(false);
            
            // 콜라이더 비활성화 (선택사항)
            var collider = controller.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // 죽음 애니메이션 재생
            var animator = controller.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IsDead", true);
                animator.SetTrigger("Die");
                hasPlayedDeathAnimation = true;
            }

            // Rigidbody가 있다면 물리 시뮬레이션 활성화 (ragdoll 효과)
            var rigidbody = controller.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }

        public void OnUpdate(EnemyAIController controller)
        {
            deathTime += Time.deltaTime;
            
            // 일정 시간 후 오브젝트 제거 (선택사항)
            if (deathTime >= 10f)
            {
                // 페이드 아웃이나 다른 이펙트 후 제거
                Object.Destroy(controller.gameObject);
            }
        }

        public void OnExit(EnemyAIController controller)
        {
            // 죽음 상태에서는 다른 상태로 전환되지 않음
        }

        public AIStateType CheckTransitions(EnemyAIController controller)
        {
            // 죽음 상태에서는 전환 없음
            return AIStateType.Dead;
        }
    }
}
