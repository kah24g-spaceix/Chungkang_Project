using UnityEngine;

public interface IPlayerInput
{
    Vector3 MoveDir { get; }
    bool IsAttackPressed { get; }
    bool IsHeavyAttackPressed { get; } // 새로 추가
    bool IsJumpPressed { get; }
}