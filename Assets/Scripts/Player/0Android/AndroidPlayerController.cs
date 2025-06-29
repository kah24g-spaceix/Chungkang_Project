using UnityEngine;

public class AndroidPlayerController : BasePlayerController
{
    private AndroidIdleState idleState;
    
    protected override void InitializeComponents()
    {
        movement = GetComponent<AndroidMovementComponent>();
        animation = GetComponent<AndroidAnimationComponent>();
        input = GetComponent<AndroidInputComponent>();
        attack = GetComponent<AndroidAttackComponent>();
    }
    
    protected override void InitializeStates()
    {
        idleState = new AndroidIdleState(movement, animation, input, stateMachine);
        stateMachine.ChangeState(idleState);
    }
    
    protected override void PerformRoll()
    {
        stateMachine.ChangeState(new AndroidRollState(movement, animation, input, stateMachine));
        lastRollTime = Time.time;
    }
    
    protected override void PerformAttack()
    {
        stateMachine.ChangeState(new AndroidAttackState(movement, animation, input, attack, stateMachine));
        lastAttackTime = Time.time;
    }
}
