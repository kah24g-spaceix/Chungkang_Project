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
    private InputComponent _input;
    private AttackComponent _attack;

    public StateMachine StateMachine => _stateMachine;

    private float _rollCooldown = 0.5f;  // 쿨타임 0.5초
    private float _lastRollTime = -Mathf.Infinity;

    private void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animation = GetComponent<AnimationComponent>();
        _input = GetComponent<InputComponent>();
        _attack = GetComponent<AttackComponent>();

        _stateMachine = new StateMachine();

        _idleState = new IdleState(_movement, _animation, _input, _stateMachine);
        _runState = new RunState(_movement, _animation, _input, _stateMachine);
        _rollState = new RollState(_movement, _animation, _input, _stateMachine);
        _attackState = new AttackState(_movement, _animation, _input, _attack, _stateMachine);
    }
    private void Start()
    {
        _stateMachine.ChangeState(_idleState);
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= _lastRollTime + _rollCooldown)
        {
            _stateMachine.ChangeState(_rollState);
            _lastRollTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _stateMachine.ChangeState(_attackState);
        }
    }
}
