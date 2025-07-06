using UnityEngine;
public interface IPlayerAnimation
{
    void SetRun(bool isRunning);
    void SetAttack(bool isAttacking);
    void SetRunSpeed(float speed);
    // 월드 이동벡터 → 로컬 이동벡터로 변환해 BlendTree에 전달
    void UpdateMovementBlend(Vector3 worldMoveDir, Transform characterTransform);
}