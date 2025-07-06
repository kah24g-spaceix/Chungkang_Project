using UnityEngine;

public class HumanInputComponent : BasePlayerInput
{
    [Header("Human Specific PC Settings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode blockKey = KeyCode.Mouse1;
    
    // Human 특화 입력들
    public bool IsSprintHeld { get; private set; }
    public bool IsBlockPressed { get; private set; }
    public bool IsBlockHeld { get; private set; }
    
    protected override void UpdateActionInput()
    {
        base.UpdateActionInput();
        
        // Human 특화 PC 입력들
        IsSprintHeld = Input.GetKey(sprintKey);
        IsBlockPressed = Input.GetKeyDown(blockKey);
        IsBlockHeld = Input.GetKey(blockKey);
    }
    
    // Human에게 유용한 PC 전용 메서드들
    public bool IsCombatModeActive()
    {
        return IsBlockHeld || IsAttackPressed;
    }
    
    public Vector2 GetAimDirection()
    {
        // 마우스 방향으로 조준
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        
        Vector3 direction = (worldPos - transform.position).normalized;
        return new Vector2(direction.x, direction.z);
    }
}
