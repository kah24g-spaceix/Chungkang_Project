using UnityEngine;
public abstract class BasePlayerController : MonoBehaviour
{
    protected IPlayerMovement movement;
    protected new IPlayerAnimation animation;
    protected IPlayerInput input;
    protected IPlayerAttack attack;
    protected StateMachine stateMachine;

    [Header("Cooldowns")]
    public float dodgeCooldown = 1f;
    public float attackCooldown = 1f;

    protected float lastDodgeTime;
    protected float lastAttackTime;

    protected virtual void Awake()
    {
        InitializeComponents();
        stateMachine = new StateMachine();
    }

    protected abstract void InitializeComponents();
    protected abstract void InitializeStates();

    protected virtual void Start()
    {
        InitializeStates();
    }

    protected virtual void Update()
    {
        stateMachine.Update(Time.deltaTime);
        movement.Move(input.MoveDir, Time.deltaTime);
        ProcessDodge();
        ProcessAttack();
    }

    private void ProcessDodge()
    {
        if (input.IsDodgePressed && CanDodge())
            PerformDodge();
    }

    private void ProcessAttack()
    {
        if (!input.IsAttackPressed) return;

        if (CanAttack())
            PerformAttack();
        else
            Debug.Log("공격 쿨다운 중...");
    }

    protected virtual bool CanDodge()
    {
        return Time.time >= lastDodgeTime + dodgeCooldown;
    }

    protected virtual bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    protected abstract void PerformDodge();
    protected abstract void PerformAttack();
}