using UnityEngine;

public class AndroidAttackState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly AndroidAttackComponent attackComponent;
    private readonly StateMachine sm;

    public AndroidAttackState(
        IPlayerMovement m,
        IPlayerAnimation a,
        IPlayerInput i,
        IPlayerAttack atkC,
        StateMachine s)
    {
        move = m;
        anim = a;
        input = i;
        attackComponent = atkC as AndroidAttackComponent;
        sm = s;
    }

    public void Enter()
    {
        // 공격은 AndroidAttackComponent에서 자체적으로 관리
    }

    public void Execute(float dt)
    {
        HandleAttackInput();
        HandleRestrictedMovement(dt);
        
        if (!attackComponent.IsAttacking)
        {
            sm.ChangeState(new AndroidIdleState(move, anim, input, sm));
        }
    }

    public void Exit()
    {
        // Exit 시 특별한 처리 없음
    }
    
    private void HandleAttackInput()
    {
        // 일반 공격 입력 (콤보)
        if (input.IsAttackPressed)
        {
            if (attackComponent.CanCombo)
            {
                // 인수 없이 호출 (수정됨)
                attackComponent.TryComboAttack();
                Debug.Log("Combo attack triggered");
            }
        }
    }
    
    private void HandleRestrictedMovement(float dt)
    {
        // 제한된 움직임 허용
        if (input.MoveDir.magnitude > 0.1f)
        {
            move.Move(input.MoveDir, dt);
            anim.SetRun(true);
        }
        else
        {
            anim.SetRun(false);
        }
    }
}
