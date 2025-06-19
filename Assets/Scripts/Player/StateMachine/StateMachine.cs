public class StateMachine
{
    private IState _currentState;
    public IState CurrentState => _currentState;

    public void ChangeState(IState newState)
    {
        if (_currentState == newState) return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
        EventBus.OnStateChanged?.Invoke();
    }

    public void Update(float deltaTime)
    {
        _currentState?.Execute(deltaTime);
    }
}
