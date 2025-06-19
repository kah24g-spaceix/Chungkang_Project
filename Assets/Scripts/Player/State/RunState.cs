using UnityEngine;

public class RunState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;

    public RunState(MovementComponent movement, AnimationComponent animation)
    {
        _movement = movement;
        _animation = animation;
    }

    public void Enter()
    {
        _animation.SetRun(true);
    }

    public void Execute(float deltaTime)
    {
        _movement.Move(deltaTime);
    }

    public void Exit()
    {
        _animation.SetRun(false);
    }
}
