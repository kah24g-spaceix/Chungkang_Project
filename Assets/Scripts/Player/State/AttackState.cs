using UnityEngine;

public class AttackState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;
    private readonly AttackComponent _attack;
    private readonly StateMachine _stateMachine;

    private float _attackDuration = 0.5f;
    private float _elapsedTime;

    public AttackState(MovementComponent movement, AnimationComponent animation, AttackComponent attack, StateMachine stateMachine)
    {
        _movement = movement;
        _animation = animation;
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
        _movement.Move(deltaTime);

        if (_elapsedTime >= _attackDuration)
        {
            if (_movement.CurrentSpeed > 0f)
                _stateMachine.ChangeState(new RunState(_movement, _animation));
            else
                _stateMachine.ChangeState(new IdleState(_movement, _animation));
        }
    }

    public void Exit() { }
}
