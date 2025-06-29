public class HumanAttackState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerAttack atk;
    private readonly StateMachine sm;
    private float elapsed;
    private readonly float dur;

    public HumanAttackState(IPlayerMovement m, IPlayerAnimation a, IPlayerAttack atkC, StateMachine s, float duration)
    {
        move = m; anim = a; atk = atkC; sm = s; dur = duration;
    }

    public void Enter()
    {
        move.CanMove = false;
        anim.SetAttack(true);
        atk.Attack();
        elapsed = 0f;
    }

    public void Execute(float dt)
    {
        elapsed += dt;
        if (elapsed >= dur)
            sm.ChangeState(new HumanIdleState(move, anim, null, sm));
    }

    public void Exit()
    {
        move.CanMove = true;
        anim.SetAttack(false);
    }
}
