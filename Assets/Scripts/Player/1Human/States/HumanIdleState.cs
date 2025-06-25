public class HumanIdleState : IState
{
    private readonly HumanMovementComponent move;
    private readonly HumanAnimationComponent anim;
    private readonly HumanInputComponent input;
    private readonly StateMachine sm;
    public HumanIdleState(HumanMovementComponent m, HumanAnimationComponent a, HumanInputComponent i, StateMachine s)
    {
        move = m; anim = a; input = i; sm = s;
    }
    public void Enter() => anim.SetRun(false);
    public void Execute(float dt)
    {
        if (input.MoveDir.magnitude > 0f)
            sm.ChangeState(new HumanRunState(move, anim, input, sm));
    }
    public void Exit() { }
}