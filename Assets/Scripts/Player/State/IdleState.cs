using UnityEngine;

public class IdleState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;

    public IdleState(MovementComponent movement, AnimationComponent animation)
    {
        _movement = movement;
        _animation = animation;
    }

    public void Enter()
    {
    }

    public void Execute(float deltaTime)
    {
        _movement.Move(deltaTime);
    }

    public void Exit()
    {
    }
}
