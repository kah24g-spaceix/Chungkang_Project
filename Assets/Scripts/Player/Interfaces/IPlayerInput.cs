using UnityEngine;

public interface IPlayerInput
{
    Vector3 MoveDir { get; }
    bool IsAttackPressed { get; }
    bool IsHeavyAttackPressed { get; }
    bool IsDodgePressed { get; }
    bool IsInteractPressed { get; } 
}