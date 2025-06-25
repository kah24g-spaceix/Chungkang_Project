public interface IState
{
    void Enter();
    void Execute(float deltaTime);
    void Exit();
}

public class StateMachine
{
    private IState _currentState;

    public void ChangeState(IState newState)
    {
        if (_currentState == newState) return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update(float deltaTime)
    {
        _currentState?.Execute(deltaTime);
    }
}