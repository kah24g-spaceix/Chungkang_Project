using UnityEngine;

public class BasePlayerInput : MonoBehaviour, IPlayerInput
{
    [Header("Input Settings")]
    [SerializeField] protected KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] protected KeyCode jumpKey = KeyCode.Space;
    
    public Vector3 MoveDir { get; private set; }
    public bool IsAttackPressed { get; private set; }
    public bool IsHeavyAttackPressed { get; private set; } // 아이템 시스템에서 제어
    public bool IsJumpPressed { get; private set; }
    
    protected virtual void Update()
    {
        UpdateMovementInput();
        UpdateActionInput();
    }
    
    protected virtual void UpdateMovementInput()
    {
        MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveDir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * MoveDir;
    }
    
    protected virtual void UpdateActionInput()
    {
        IsAttackPressed = Input.GetKeyDown(attackKey);
        IsJumpPressed = Input.GetKeyDown(jumpKey);
        
        // 강화공격은 인벤토리/아이템 시스템에서 결정
        // IsHeavyAttackPressed는 여기서 직접 설정하지 않음
    }
    
    // 아이템 시스템에서 호출할 메서드
    public void SetHeavyAttackMode(bool enabled)
    {
        IsHeavyAttackPressed = enabled && IsAttackPressed;
    }
}
