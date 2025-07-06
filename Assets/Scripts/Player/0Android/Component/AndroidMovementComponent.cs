using UnityEngine;

public class AndroidMovementComponent : BasePlayerMovement
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 720f; // 초당 회전 속도 (도)
    [SerializeField] private bool useMouseRotation = true;

    private float originalSpeed;
    private float currentSpeedMultiplier = 1f;
    private AndroidInputComponent androidInput;
    protected override void Awake()
    {
        base.Awake();
        originalSpeed = MoveSpeed;
        androidInput = GetComponent<AndroidInputComponent>();
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
        MoveSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        currentSpeedMultiplier = 1f;
        MoveSpeed = originalSpeed;
    }

    private void RotateToMouseDirection(float deltaTime)
    {
        if (!useMouseRotation || androidInput == null) return;

        Vector3 dir = androidInput.MouseDirection;
        if (dir.sqrMagnitude < 0.01f) return;

        var target = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            target,
            rotationSpeed * deltaTime
        );
    }

    public void MoveForwardInAttack(float speed, float deltaTime)
    {
        ApplyMovement(transform.forward * speed * deltaTime);
    }

    public void DodgeMove(Vector3 direction, float speed, float deltaTime)
    {
        ApplyMovement(direction.normalized * speed * deltaTime);
    }

    public void DashMove(Vector3 direction, float speed, float deltaTime)
    {
        if (rb != null)
            rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z) * speed;
        else
            ApplyMovement(direction.normalized * speed * deltaTime);
    }

    public override void Move(Vector3 moveDirection, float deltaTime)
    {
        base.Move(moveDirection, deltaTime);   // 위치 이동 + 애니메이션 파라미터 갱신
        RotateToMouseDirection(deltaTime);     // 마우스 기준 회전
    }
    private void ApplyMovement(Vector3 movement)
    {
        if (rb != null)
        {
            rb.MovePosition(transform.position + movement);
        }
        else
        {
            transform.position += movement;
        }
    }
}
