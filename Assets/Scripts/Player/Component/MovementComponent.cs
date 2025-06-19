using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementComponent : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 20f;
    public float rollSpeed = 8f;
    public float rollDuration = 0.3f;
    public float verticalBoost = 0.01f;
    public float colliderRecoverSpeed = 10f;

    private float moveSpeedModifier = 0f; // 아이템 버프로 증가하는 값

    private float _currentSpeed;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private Vector3 _direction;
    private bool _isRolling;
    private float _rollTimer;
    private float _originalHeight;

    public bool IsInvincible { get; private set; }
    public Vector3 MoveDirection => _direction;
    public float CurrentSpeed => _currentSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        _originalHeight = _collider.height;
        EventBus.OnMoveInputChanged += OnMoveInput;
    }

    private void OnDestroy()
    {
        EventBus.OnMoveInputChanged -= OnMoveInput;
    }

    private void OnMoveInput(float magnitude)
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        _direction = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0)
                     * new Vector3(h, 0, v).normalized;
    }

    public void Move(float deltaTime)
    {
        float totalMoveSpeed = moveSpeed + moveSpeedModifier;

        _currentSpeed = _direction.magnitude * totalMoveSpeed;
        if (_currentSpeed > 0f)
        {
            Vector3 move = _direction * totalMoveSpeed * deltaTime;
            _rigidbody.MovePosition(_rigidbody.position + move);

            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * deltaTime);
        }
    }

    public void StartRoll(Vector3 direction)
    {
        _isRolling = true;
        _rollTimer = 0f;
        _direction = direction;

        _collider.height = _originalHeight * 0.5f;
    }

    private void EndRoll()
    {
        _isRolling = false;
        _collider.height = _originalHeight;
    }

    private void FixedUpdate()
    {
        if (_isRolling)
        {
            _rollTimer += Time.fixedDeltaTime;
            if (_rollTimer >= rollDuration)
            {
                EndRoll();
            }
        }
        _collider.height = Mathf.Lerp(_collider.height, _originalHeight, Time.fixedDeltaTime * colliderRecoverSpeed);
    }

    public void RollMove(Vector3 direction, float deltaTime)
    {
        StartRoll(direction);
        Vector3 move = direction.normalized * rollSpeed * deltaTime;
        Vector3 targetPosition = _rigidbody.position + move;
        _rigidbody.MovePosition(targetPosition);
    }

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }

    public void ApplyMoveSpeedBuff(float buffAmount)
    {
        moveSpeed += buffAmount;
        EventBus.OnMoveSpeedChanged?.Invoke(moveSpeed);
    }

    public void RemoveMoveSpeedBuff(float amount)
    {
        moveSpeedModifier -= amount;
        if (moveSpeedModifier < 0f) moveSpeedModifier = 0f;
    }
}
