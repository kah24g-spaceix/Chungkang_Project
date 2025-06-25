public class AndroidRunState : IState
{
    private readonly AndroidMovementComponent move;
    private readonly AndroidAnimationComponent anim;
    private readonly AndroidInputComponent input;
    private readonly StateMachine sm;
    public AndroidRunState(AndroidMovementComponent m, AndroidAnimationComponent a, AndroidInputComponent i, StateMachine s)
    {
        move = m; anim = a; input = i; sm = s;
    }
    public void Enter()
    {
        anim.SetRun(true);
        anim.SetRunSpeed(move.moveSpeed / 5);
    }
    public void Execute(float dt)
    {
        move.Move(input.MoveDir, dt);
        if (input.MoveDir.magnitude <= 0f)
            sm.ChangeState(new AndroidIdleState(move, anim, input, sm));
    }
    public void Exit() => anim.SetRun(false);
}