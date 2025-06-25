public class AndroidAttackState : IState
{
    private readonly AndroidMovementComponent move;
    private readonly AndroidAnimationComponent anim;
    private readonly AndroidInputComponent input;
    private readonly AndroidAttackComponent atk;
    private readonly StateMachine sm;
    private float elapsed;
    private readonly float dur;
    public AndroidAttackState(
        AndroidMovementComponent m,
        AndroidAnimationComponent a,
        AndroidInputComponent i,
        AndroidAttackComponent atkC,
        StateMachine s)
    {
        move = m;
        anim = a;
        input = i;
        atk = atkC;
        sm = s;
        dur = 1f;
    }
    public void Enter()
    {
        anim.SetAttack(true);
        atk.Attack();
        elapsed = 0f;
    }
    public void Execute(float dt)
    {
        elapsed += dt;
        Move(dt);
        if (elapsed >= dur)
        {
            atk.AttackEnd();
            sm.ChangeState(new AndroidIdleState(move, anim, input, sm));
        }
    }
    public void Exit()
    {
        anim.SetAttack(false);
    }
    private void Move(float dt)
    {
        move.Move(input.MoveDir, dt);
        if (input.MoveDir.magnitude > 0f) anim.SetRun(true);
        else anim.SetRun(false);
    }
}
