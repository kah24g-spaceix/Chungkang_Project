using UnityEngine;

public class AndroidMovementComponent : BasePlayerMovement
{
    private float originalSpeed;
    private float currentSpeedMultiplier = 1f;
    
    protected override void Awake()
    {
        base.Awake();
        // Android 특화 초기화
        originalSpeed = moveSpeed; // BasePlayerMovement의 speed 사용
    }
    
    public void SetMovementSpeed(float newSpeed)
    {
        // BasePlayerMovement의 speed 값 업데이트
        moveSpeed = newSpeed;
    }
    
    public float GetMovementSpeed()
    {
        return moveSpeed; // BasePlayerMovement의 speed 반환
    }
    
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
        SetMovementSpeed(originalSpeed * multiplier);
    }
    
    public void ResetSpeed()
    {
        SetMovementSpeed(originalSpeed);
        currentSpeedMultiplier = 1f;
    }
    
    // Android 특화 이동 로직 (필요시 오버라이드)
    public override void Move(Vector3 moveDirection, float deltaTime)
    {
        base.Move(moveDirection, deltaTime);
        
        // Android 특화 추가 로직이 있다면 여기에 구현
    }
}
