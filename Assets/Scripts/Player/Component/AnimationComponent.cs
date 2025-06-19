using UnityEngine;

public class AnimationComponent : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetIdle(bool isIdle)
    {
        _animator.SetBool("Idle", isIdle);
    }
    public void SetRun(bool isRunning)
    {
        _animator.SetBool("Run", isRunning);
    }

    public void SetAttack(bool isAttacking)
    {
        _animator.SetBool("Attack", isAttacking);
    }

    public void SetRoll(bool isRolling)
    {
        _animator.SetBool("Roll", isRolling);
    }
}
