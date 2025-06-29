public class AndroidRunState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly StateMachine sm;

    public AndroidRunState(IPlayerMovement m, IPlayerAnimation a, IPlayerInput i, StateMachine s)
    {
        move = m; anim = a; input = i; sm = s;
    }

    public void Enter()
    {
        anim.SetRun(true);
        anim.SetRunSpeed(move.MoveSpeed / 5);
    }

    public void Execute(float dt)
    {
        move.Move(input.MoveDir, dt);
        if (input.MoveDir.magnitude <= 0f)
            sm.ChangeState(new AndroidIdleState(move, anim, input, sm));
    }

    public void Exit() => anim.SetRun(false);
}
