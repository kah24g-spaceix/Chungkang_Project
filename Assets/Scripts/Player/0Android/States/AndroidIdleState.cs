public class AndroidIdleState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly StateMachine sm;

    public AndroidIdleState(IPlayerMovement m, IPlayerAnimation a, IPlayerInput i, StateMachine s)
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
