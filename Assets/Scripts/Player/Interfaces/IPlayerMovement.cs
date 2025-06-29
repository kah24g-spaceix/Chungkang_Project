using UnityEngine;

public interface IPlayerMovement
{
    float MoveSpeed { get; set; }
    float RotateSpeed { get; set; }
    float RollSpeed { get; set; }
    bool CanMove { get; set; }
    bool IsInvincible { get; set; }
    
    void Move(Vector3 direction, float deltaTime);
    void Roll(float deltaTime);
}
