using UnityEngine;

public interface IPlayerMovement
{
    float MoveSpeed { get; set; }
    bool CanMove { get; set; }
    bool IsInvincible { get; set; }
    
    void Move(Vector3 direction, float deltaTime);
    void Dodge(Vector3 direction);
}
