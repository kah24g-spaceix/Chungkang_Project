using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    protected IPlayerMovement movement;
    protected IPlayerAnimation animation;
    protected IPlayerInput input;
    protected IPlayerAttack attack;
    protected StateMachine stateMachine;
    
    [Header("Cooldowns")]
    public float rollCooldown = 1f;
    public float attackCooldown = 1f;
    
    protected float lastRollTime;
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
        
        HandleInput();
    }
    
    protected virtual void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanRoll())
        {
            PerformRoll();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && CanAttack())
        {
            PerformAttack();
        }
    }
    
    protected virtual bool CanRoll()
    {
        return Time.time >= lastRollTime + rollCooldown;
    }
    
    protected virtual bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }
    
    protected abstract void PerformRoll();
    protected abstract void PerformAttack();
}
