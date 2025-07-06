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
        // animation 변수 사용 (IPlayerAnimation 인터페이스)
        idleState = new HumanIdleState(movement, animation, input, stateMachine);
        stateMachine.ChangeState(idleState);
    }
    
    protected override void PerformAttack()
    {
        // animation 변수 사용 (IPlayerAnimation 인터페이스)
        stateMachine.ChangeState(new HumanAttackState(movement, animation, attack, stateMachine, attackDuration));
        lastAttackTime = Time.time;
    }

    protected override void PerformDodge()
    {
        throw new System.NotImplementedException();
    }
}
