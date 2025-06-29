using UnityEngine;

public class BasePlayerAnimation : MonoBehaviour, IPlayerAnimation
{
    protected Animator animator;
    
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public virtual void SetRun(bool isRunning)
    {
        animator.SetBool("Run", isRunning);
    }
    
    public virtual void SetRoll(bool isRolling)
    {
        animator.SetBool("Roll", isRolling);
    }
    
    public virtual void SetAttack(bool isAttacking)
    {
        animator.SetBool("Attack", isAttacking);
    }
    
    public virtual void SetRunSpeed(float speed)
    {
        animator.SetFloat("RunSpeed", speed);
    }
}
