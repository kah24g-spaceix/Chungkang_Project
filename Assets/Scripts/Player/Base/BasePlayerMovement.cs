using UnityEngine;

public class BasePlayerMovement : MonoBehaviour, IPlayerMovement
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float dashDistance = 5f;
    [SerializeField] protected bool canMove = true;
    protected Rigidbody rb;

    // IPlayerAnimation 참조
    protected IPlayerAnimation anim;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public bool CanMove
    {
        get => canMove;
        set => canMove = value;
    }

    public bool IsInvincible { get; set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogWarning($"[{nameof(BasePlayerMovement)}] Rigidbody 컴포넌트를 찾을 수 없습니다.", this);

        // 애니메이션 컴포넌트 가져오기
        anim = GetComponent<IPlayerAnimation>();
        if (anim == null)
            Debug.LogWarning($"[{nameof(BasePlayerMovement)}] IPlayerAnimation 구현체를 찾을 수 없습니다.", this);
    }

    public virtual void Move(Vector3 direction, float deltaTime)
    {
        if (!CanMove || direction.sqrMagnitude < 0.0001f)
            return;

        // 이동만 처리
        Vector3 displacement = direction.normalized * moveSpeed * deltaTime;
        if (rb != null)
            rb.MovePosition(rb.position + displacement);
        else
            transform.position += displacement;

        // 이동 벡터를 애니메이션 파라미터로 업데이트
        anim?.UpdateMovementBlend(direction, transform);
    }

    public virtual void Dodge(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            direction = transform.forward;

        IsInvincible = true;
        Vector3 dashDir = direction.normalized * dashDistance;
        if (rb != null)
            rb.MovePosition(rb.position + dashDir);
        else
            transform.position += dashDir;
        IsInvincible = false;
    }
}