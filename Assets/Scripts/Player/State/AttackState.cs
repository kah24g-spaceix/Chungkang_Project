public class AttackState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;
    private readonly InputComponent _input;
    private readonly AttackComponent _attack;
    private readonly StateMachine _stateMachine;

    private float _attackDuration = 0.5f;
    private float _elapsedTime;

    public AttackState(MovementComponent movement, AnimationComponent animation, InputComponent input, AttackComponent attack, StateMachine stateMachine)
    {
        _movement = movement;
        _animation = animation;
        _input = input;
        _attack = attack;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animation.SetAttack(true);
        _attack.DoAttack();
        _elapsedTime = 0f;
    }

    public void Execute(float deltaTime)
    {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= _attackDuration)
        {
            _stateMachine.ChangeState(new IdleState(_movement, _animation, _input, _stateMachine));
        }
    }

    public void Exit()
    {
        _animation.SetAttack(false);
    }
}
