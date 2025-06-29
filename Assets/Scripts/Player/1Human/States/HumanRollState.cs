using UnityEngine;

public class HumanRollState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly StateMachine sm;
    private float elapsed;
    private readonly float rollDur = 0.5f;

    public HumanRollState(IPlayerMovement m, IPlayerAnimation a, IPlayerInput i, StateMachine s)
    {
        move = m; anim = a; input = i; sm = s;
    }

    public void Enter()
    {
        move.CanMove = false;
        move.IsInvincible = true;
        anim.SetRoll(true);
        elapsed = 0f;
    }

    public void Execute(float dt)
    {
        elapsed += dt;
        move.Roll(dt);
        if (elapsed >= rollDur)
        {
            sm.ChangeState(new HumanIdleState(move, anim, input, sm));
        }
    }

    public void Exit()
    {
        move.CanMove = true;
        move.IsInvincible = false;
        anim.SetRoll(false);
    }
}
