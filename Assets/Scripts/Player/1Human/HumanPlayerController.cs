using UnityEngine;

public class HumanPlayerController : BasePlayerController
{
    public float attackDuration = 0.7f;
    private HumanIdleState idleState;
    
    protected override void InitializeComponents()
    {
        movement = GetComponent<HumanMovementComponent>();
        animation = GetComponent<HumanAnimationComponent>();
        input = GetComponent<HumanInputComponent>();
        attack = GetComponent<HumanAttackComponent>();
    }
    
    protected override void InitializeStates()
    {
        idleState = new HumanIdleState(movement, animation, input, stateMachine);
        stateMachine.ChangeState(idleState);
    }
    
    protected override void PerformRoll()
    {
        stateMachine.ChangeState(new HumanRollState(movement, animation, input, stateMachine));
        lastRollTime = Time.time;
    }
    
    protected override void PerformAttack()
    {
        stateMachine.ChangeState(new HumanAttackState(movement, animation, attack, stateMachine, attackDuration));
        lastAttackTime = Time.time;
    }
}
