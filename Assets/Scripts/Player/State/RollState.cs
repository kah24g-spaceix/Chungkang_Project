using UnityEngine;

public class RollState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;
    private readonly StateMachine _stateMachine;

    private Vector3 _rollDirection;
    private float _rollTime;
    private readonly float _rollDuration = 0.4f;

    public RollState(MovementComponent movement, AnimationComponent animation, StateMachine stateMachine)
    {
        _movement = movement;
        _animation = animation;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _rollDirection = _movement.MoveDirection == Vector3.zero ? _movement.transform.forward : _movement.MoveDirection;
        _animation.SetRoll(true);
        _movement.SetInvincible(true);
        _rollTime = 0f;
    }

    public void Execute(float deltaTime)
    {
        _rollTime += deltaTime;
        _movement.RollMove(_rollDirection, deltaTime);

        if (_rollTime >= _rollDuration)
        {
            _movement.SetInvincible(false);
            _stateMachine.ChangeState(new IdleState(_movement, _animation));
        }
    }

    public void Exit()
    {
        _movement.SetInvincible(false);
        _animation.SetRoll(false);
    }
}
