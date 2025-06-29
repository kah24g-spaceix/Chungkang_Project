using UnityEngine;

public class BasePlayerMovement : MonoBehaviour, IPlayerMovement
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 20f;
    [SerializeField] protected float rollSpeed = 5f;
    [SerializeField] protected bool canMove = true;
    protected Rigidbody rb;
    
    public float MoveSpeed 
    { 
        get => moveSpeed; 
        set => moveSpeed = value; 
    }
    
    public float RotateSpeed 
    { 
        get => rotateSpeed; 
        set => rotateSpeed = value; 
    }
    
    public float RollSpeed 
    { 
        get => rollSpeed; 
        set => rollSpeed = value; 
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
    }
    
    public virtual void Move(Vector3 direction, float deltaTime)
    {
        if (!CanMove || direction.magnitude == 0) return;
        
        rb.MovePosition(rb.position + direction * moveSpeed * deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed * deltaTime);
    }
    
    public virtual void Roll(float deltaTime)
    {
        rb.MovePosition(rb.position + transform.forward * rollSpeed * deltaTime);
    }
}
