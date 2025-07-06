using UnityEngine;

public class BasePlayerAnimation : MonoBehaviour, IPlayerAnimation
{
    public Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning($"[{nameof(BasePlayerAnimation)}] Animator 컴포넌트를 찾을 수 없습니다.", this);
    }
    
    public virtual void SetRun(bool isRunning)
    {
        animator.SetBool("Run", isRunning);
    }

    public virtual void SetAttack(bool isAttacking)
    {
        animator.SetBool("Attack", isAttacking);
    }

    public virtual void SetRunSpeed(float speed)
    {
        animator.SetFloat("RunSpeed", speed);
    }

    // 월드 방향을 로컬로 변환해 BlendTree 파라미터에 넣는다
    public virtual void UpdateMovementBlend(Vector3 worldMoveDir, Transform t)
    {
        if (animator == null) return;

        // 캐릭터 로컬 좌표계로 변환
        Vector3 localDir = t.InverseTransformDirection(worldMoveDir);
        animator.SetFloat("MovementX", localDir.x);
        animator.SetFloat("MovementY", localDir.z);
    }
}