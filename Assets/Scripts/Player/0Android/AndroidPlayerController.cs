using Unity.VisualScripting;
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
        idleState = new AndroidIdleState(movement, animation, input, attack, stateMachine);
        stateMachine.ChangeState(idleState);
    }

    protected override void PerformAttack()
    {
        stateMachine.ChangeState(new AndroidAttackState(movement, animation, input, attack, stateMachine));
        lastAttackTime = Time.time;
    }

    protected override void PerformDodge()
    {
        stateMachine.ChangeState(new AndroidDodgeState(movement, animation, input, attack, stateMachine));
        lastDodgeTime = Time.time;
    }

    private void HandleDodgeInput()
    {
        AndroidInputComponent androidInput = input as AndroidInputComponent;

        if (androidInput != null && androidInput.IsDodgePressed && CanDodge())
        {
            Debug.Log("Dodge input detected!");
            PerformDodge();  // 회피 상태로 전환
        }
    }

    protected override bool CanDodge()
    {
        // 공격 중이거나 이미 회피 중이라면 회피 불가
        return !IsAttacking() && !IsDodging() && Time.time >= lastDodgeTime + dodgeCooldown;
    }

    private bool IsAttacking()
    {
        // 현재 상태가 공격 상태인지 검사
        return stateMachine.CurrentState is AndroidAttackState;
    }

    private bool IsDodging()
    {
        // 현재 상태가 회피 상태인지 검사
        return stateMachine.CurrentState is AndroidDodgeState;
    }

    protected override void Update()
    {
        base.Update();
        HandleDodgeInput();
    }
}
