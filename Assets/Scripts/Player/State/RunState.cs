public class RunState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;
    private readonly InputComponent _input;
    private readonly StateMachine _stateMachine;

    public RunState(MovementComponent movement, AnimationComponent animation, InputComponent input, StateMachine stateMachine)
    {
        _movement = movement;
        _animation = animation;
        _input = input;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animation.SetRun(true);
    }

    public void Execute(float deltaTime)
    {
        _movement.Move(_input.MoveDirection, deltaTime);
        if (_input.MoveDirection.magnitude <= 0f)
        {
            _stateMachine.ChangeState(new IdleState(_movement, _animation, _input, _stateMachine));
        }
    }

    public void Exit()
    {
        _animation.SetRun(false);
    }
}
