// AndroidDodgeState.cs
using UnityEngine;

public class AndroidDodgeState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly StateMachine sm;

    private Vector3 dashDirection;
    private float elapsed;
    private readonly float dashDuration = 0.3f;
    private readonly float dashSpeed = 15f;

    public AndroidDodgeState(
        IPlayerMovement m,
        IPlayerAnimation a,
        IPlayerInput i,
        IPlayerAttack atkC,
        StateMachine s)
    {
        move = m;
        anim = a;
        input = i;
        sm = s;
    }

    public void Enter()
    {
        // 입력에서 대쉬 방향만 가져와서
        dashDirection = input.MoveDir;
        if (dashDirection.sqrMagnitude < 0.1f)
            dashDirection = Vector3.forward;

        elapsed = 0f;
        Debug.Log($"Dash started! Direction: {dashDirection}");
        // 애니메이션 파라미터 제거: SetDodge 호출하지 않습니다.
    }

    public void Execute(float dt)
    {
        elapsed += dt;
        // 물리 이동만
        (move as AndroidMovementComponent)?.DodgeMove(dashDirection, dashSpeed, dt);

        if (elapsed >= dashDuration)
        {
            // 끝나면 Idle 로
            sm.ChangeState(new AndroidIdleState(move, anim, input, null, sm));
        }
    }

    public void Exit()
    {
        Debug.Log("Dash ended!");
        // 애니메이션 파라미터 제거: SetDodge false 호출하지 않습니다.
    }
}
