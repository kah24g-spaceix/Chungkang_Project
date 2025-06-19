using NUnit.Framework;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public int maxEP = 50;
    public int currentEP;

    public int level = 1;
    public int currentExp = 0;
    public int expToLevelUp = 100;

    public int attackPower = 10;

    public float epRecoveryRate = 5f;

    private void Start()
    {
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) currentHP = 0;

        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
    }

    public void UseEP(int amount)
    {
        if (currentEP - amount < 0) return;
        currentEP -= amount;

        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
    }
    public void RecoverEP(int amount)
    {
        currentEP += amount;
        if (currentEP > maxEP) currentEP = maxEP;
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevelUp) LevelUp();
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
    }

    void LevelUp()
    {
        level++;
        currentExp = 0;
        maxHP *= 1 + (10 / 100);
        maxEP *= 1 + (10 / 100);

        currentHP = maxHP;
        currentEP = maxEP;

        attackPower *= 1 + (10 / 100);

        expToLevelUp = Mathf.RoundToInt(expToLevelUp * 1.5f);

        EventBus.OnLevelUp?.Invoke(level);
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
    }
    void LevelDown()
    {
        level--;
        maxHP *= 1 - (10 / 100);
        maxEP *= 1 - (10 / 100);

        attackPower *= 1 - (10 / 100);

        expToLevelUp = Mathf.RoundToInt(expToLevelUp / 1.5f);

        EventBus.OnLevelUp?.Invoke(level);
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
    }
}
