public class AndroidIdleState : IState
{
    private readonly AndroidMovementComponent move;
    private readonly AndroidAnimationComponent anim;
    private readonly AndroidInputComponent input;
    private readonly StateMachine sm;
    public AndroidIdleState(AndroidMovementComponent m, AndroidAnimationComponent a, AndroidInputComponent i, StateMachine s)
    {
        move = m; anim = a; input = i; sm = s;
    }
    public void Enter() => anim.SetRun(false);
    public void Execute(float dt)
    {
        if (input.MoveDir.magnitude > 0f)
            sm.ChangeState(new AndroidRunState(move, anim, input, sm));
    }
    public void Exit() { }
}