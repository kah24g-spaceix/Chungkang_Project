using UnityEngine;
public class AndroidIdleState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly IPlayerAttack attack;
    private readonly StateMachine sm;

    public AndroidIdleState(IPlayerMovement m, IPlayerAnimation a, IPlayerInput i, IPlayerAttack atkC, StateMachine s)
    {
        move = m;
        anim = a;
        input = i;
        attack = atkC;
        sm = s;
    }

    public void Enter()
    {
        Debug.Log("IdleState Enter");
    }

    public void Execute(float dt)
    {
        if (input.MoveDir.magnitude > 0f)
        {
            sm.ChangeState(new AndroidRunState(move, anim, input, attack, sm));
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
        Debug.Log("IdleState Exit");
    }
}
