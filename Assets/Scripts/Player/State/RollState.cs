using UnityEngine;

public class RollState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;
    private readonly InputComponent _input;
    private readonly StateMachine _stateMachine;

    private float _rollTime;
    private readonly float _rollDuration = 0.4f;

    public RollState(MovementComponent movement, AnimationComponent animation,InputComponent input, StateMachine stateMachine)
    {
        _movement = movement;
        _animation = animation;
        _stateMachine = stateMachine;
        _input = input;
    }

    public void Enter()
    {
        _animation.SetRoll(true);
        _movement.SetInvincible(true);
        _movement.StartRoll();
        _rollTime = 0f;
    }

    public void Execute(float deltaTime)
    {
        _rollTime += deltaTime;
        _movement.RollMove(_input.MoveDirection, _rollTime);

        if (_rollTime >= _rollDuration)
        {
            _movement.SetInvincible(false);
            _stateMachine.ChangeState(new IdleState(_movement, _animation, _input, _stateMachine));
        }
    }

    public void Exit()
    {
        _animation.SetRoll(false);
        _movement.EndRoll();
    }
}
