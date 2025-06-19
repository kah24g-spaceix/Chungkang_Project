using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementComponent : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 20f;
    public float rollSpeed = 0.1f;
    public float rollDuration = 0.3f;
    public float colliderRecoverSpeed = 20f;

    private float moveSpeedModifier = 0f;
    private float _currentSpeed;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private bool _isRolling;
    private float _rollTimer;
    private float _originalHeight;

    public bool IsInvincible { get; private set; }
    public float CurrentSpeed => _currentSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _originalHeight = _collider.height;
    }

    public void Move(Vector3 direction, float deltaTime)
    {
        float totalMoveSpeed = moveSpeed + moveSpeedModifier;
        _currentSpeed = direction.magnitude * totalMoveSpeed;

        if (_currentSpeed > 0f)
        {
            Vector3 move = direction * totalMoveSpeed * deltaTime;
            _rigidbody.MovePosition(_rigidbody.position + move);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * deltaTime);
        }
    }

    public void RollMove(Vector3 direction, float deltaTime)
    {
        if (direction.normalized == Vector3.zero)
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            direction = forward.normalized;
        }
        Vector3 move = deltaTime * rollSpeed * direction.normalized;
        Vector3 targetPosition = _rigidbody.position + move;
        _rigidbody.MovePosition(targetPosition);
    }

    public void StartRoll()
    {
        _isRolling = true;
        _rollTimer = 0f;
        _collider.height = _originalHeight * 0.5f;
    }

    public void EndRoll()
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

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }

    public void ApplyMoveSpeedBuff(float buffAmount)
    {
        moveSpeedModifier += buffAmount;
    }

    public void RemoveMoveSpeedBuff(float amount)
    {
        moveSpeedModifier -= amount;
        if (moveSpeedModifier < 0f) moveSpeedModifier = 0f;
    }
}
