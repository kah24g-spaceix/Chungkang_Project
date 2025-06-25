public interface IPlayerStats
{
    void TakeDamage(int damage);
    void UseEP(int amount);
    void RecoverEP(int amount);
    void GainExp(int amount);
    void AddMaxHP(int hp);
    void AddMaxEP(int ep);
    void AddAttackPower(int power);
    void AddSpeed(float speed);
    void LevelUp();
    void LevelDown();
}