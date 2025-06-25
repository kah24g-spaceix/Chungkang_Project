public class AndroidRollState : IState
{
    private readonly AndroidMovementComponent move;
    private readonly AndroidAnimationComponent anim;
    private readonly AndroidInputComponent input;
    private readonly StateMachine sm;
    private float elapsed;
    private readonly float rollDur = 0.5f;
    public AndroidRollState(AndroidMovementComponent m, AndroidAnimationComponent a, AndroidInputComponent i, StateMachine s)
    {
        move = m; anim = a; sm = s; input = i;
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
            sm.ChangeState(new AndroidIdleState(move, anim, input, sm));
        }
    }
    public void Exit()
    {
        move.CanMove = true;
        move.IsInvincible = false;
        anim.SetRoll(false);
    }
}