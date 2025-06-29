public interface IPlayerAnimation
{
    void SetRun(bool isRunning);
    void SetRoll(bool isRolling);
    void SetAttack(bool isAttacking);
    void SetRunSpeed(float speed);  // 선택적 구현
}
