// AndroidRunState.cs
using UnityEngine;

public class AndroidRunState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly IPlayerAttack attack;
    private readonly StateMachine sm;

    public AndroidRunState(
        IPlayerMovement m,
        IPlayerAnimation a,
        IPlayerInput i,
        IPlayerAttack atkC,
        StateMachine s)
    {
        move = m;
        anim = a;
        input = i;
        attack = atkC;
        sm = s;
    }

    public void Enter()
    {
        anim.SetRun(true);
        anim.SetRunSpeed(move.MoveSpeed / 5f);
    }

    public void Execute(float dt)
    {
        // 1) 이동 + 자동으로 BlendTree 파라미터 업데이트
        move.Move(input.MoveDir, dt);

        // 2) 상태 전환
        if (input.MoveDir.sqrMagnitude <= 0f)
        {
            sm.ChangeState(new AndroidIdleState(move, anim, input, attack, sm));
            return;
        }
        if (input.IsAttackPressed)
        {
            sm.ChangeState(new AndroidAttackState(move, anim, input, attack, sm));
            return;
        }
        if (input.IsDodgePressed)
        {
            sm.ChangeState(new AndroidDodgeState(move, anim, input, attack, sm));
            return;
        }
    }

    public void Exit()
    {
        anim.SetRun(false);
    }
}
