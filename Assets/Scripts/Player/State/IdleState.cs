using UnityEngine;

public class IdleState : IState
{
    private readonly MovementComponent _movement;
    private readonly AnimationComponent _animation;
    private readonly InputComponent _input;
    private readonly StateMachine _stateMachine;

    public IdleState(MovementComponent movement, AnimationComponent animation, InputComponent input, StateMachine stateMachine)
    {
        _movement = movement;
        _animation = animation;
        _input = input;
        _stateMachine = stateMachine;
    }

    public void Enter() { }

    public void Execute(float deltaTime)
    {

        _movement.Move(_input.MoveDirection, deltaTime);
        
        if (_input.MoveDirection.magnitude > 0f)
        {
            _stateMachine.ChangeState(new RunState(_movement, _animation, _input, _stateMachine));
        }
    }

    public void Exit() { }
}
