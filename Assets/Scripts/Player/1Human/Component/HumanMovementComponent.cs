using UnityEngine;
public class HumanMovementComponent : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed = 20f;
    public float rollSpeed = 5f;
    public bool CanMove = true;
    private Rigidbody _rb;
    public bool IsInvincible { get; set; }
    private void Awake() => _rb = GetComponent<Rigidbody>();
    public void Move(Vector3 dir, float dt)
    {
        if (!CanMove || dir.magnitude == 0) return;
        _rb.MovePosition(_rb.position + dir * moveSpeed * dt);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotateSpeed * dt);
    }
    public void Roll(float dt)
    {
        if (!CanMove) return;
        _rb.MovePosition(_rb.position + transform.forward * rollSpeed * dt);
    }
}