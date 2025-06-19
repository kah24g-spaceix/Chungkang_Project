using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(AnimationComponent))]
public class PlayerStateMachine : MonoBehaviour
{
    private StateMachine _stateMachine;

    private IdleState _idleState;
    private RunState _runState;
    private RollState _rollState;
    private AttackState _attackState;

    private MovementComponent _movement;
    private AnimationComponent _animation;
    private AttackComponent _attack;

    public StateMachine StateMachine => _stateMachine;

    private void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animation = GetComponent<AnimationComponent>();
        _attack = GetComponent<AttackComponent>();

        _stateMachine = new StateMachine();

        _idleState = new IdleState(_movement, _animation);
        _runState = new RunState(_movement, _animation);
        _rollState = new RollState(_movement, _animation, _stateMachine);
        _attackState = new AttackState(_movement, _animation, _attack, _stateMachine);

        EventBus.OnMoveInputChanged += OnMoveInputChanged;
    }
    private void Start()
    {
        _stateMachine.ChangeState(_idleState);
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _stateMachine.ChangeState(_rollState);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _stateMachine.ChangeState(_attackState);
        }
    }

    private void OnMoveInputChanged(float magnitude)
    {
        if (_stateMachine.CurrentState is RollState)
            return;

        if (magnitude > 0f)
            _stateMachine.ChangeState(_runState);
        else
            _stateMachine.ChangeState(_idleState);
    }

    private void OnDestroy()
    {
        EventBus.OnMoveInputChanged -= OnMoveInputChanged;
    }
}
