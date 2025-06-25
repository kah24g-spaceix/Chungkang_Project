public class HumanAttackState : IState
{
    private readonly HumanMovementComponent move;
    private readonly HumanAnimationComponent anim;
    private readonly HumanAttackComponent atk;
    private readonly StateMachine sm;
    private float elapsed;
    private readonly float dur;
    public HumanAttackState(HumanMovementComponent m, HumanAnimationComponent a, HumanAttackComponent atkC, StateMachine s, float duration)
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