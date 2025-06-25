using UnityEngine;

public class AndroidPlayerStats : MonoBehaviour, IPlayerStats
{
    public int maxHP = 100;
    private int currentHP;
    public int maxEP = 50;
    private int currentEP;

    public int level = 1;
    private int currentExp = 0;
    public int expToLevelUp = 100;


    public int attackPower;
    private readonly int attackDefaultPower = 10;
    public float moveSpeed = 5f;

    public float epRecoveryRate = 5f;

    private AndroidMovementComponent _movement;
    private AndroidAttackComponent _attack;

    private void Awake()
    {
        _movement = GetComponent<AndroidMovementComponent>();
        _attack = GetComponent<AndroidAttackComponent>();
    }
    private void Start()
    {
        _movement.moveSpeed = moveSpeed;
        attackPower = attackDefaultPower;
        currentHP = maxHP;
        currentEP = maxEP;

        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
        EventBus.OnLevelUp?.Invoke(level);
        EventBus.OnAttackPowerChanged?.Invoke(attackPower);
        EventBus.OnMoveSpeedChanged?.Invoke(moveSpeed);
    }

    public void TakeDamage(int damage)
    {
        if (_movement.IsInvincible) return;
        
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

    public void AddMaxHP(int hp)
    {
        currentHP += hp;
        maxHP += hp;
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
    }
    public void AddMaxEP(int ep)
    {
        currentHP += ep;
        maxHP += ep;
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
    }
    public void AddAttackPower(int power)
    {
        attackPower += power;
        EventBus.OnAttackPowerChanged?.Invoke(attackPower);
    }
    public void AddSpeed(float speed)
    {
        moveSpeed += speed;
        _movement.moveSpeed = moveSpeed;

        EventBus.OnMoveSpeedChanged?.Invoke(moveSpeed);
    }
    public void LevelUp()
    {
        level++;
        currentExp = 0;
        maxHP *= 1 + (10 / 100);
        maxEP *= 1 + (10 / 100);

        currentHP = maxHP;
        currentEP = maxEP;

        attackPower += attackDefaultPower * 1 + (10 / 100);

        expToLevelUp = Mathf.RoundToInt(expToLevelUp * 1.5f);

        EventBus.OnLevelUp?.Invoke(level);
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
        EventBus.OnAttackPowerChanged?.Invoke(attackPower);
    }
    public void LevelDown()
    {
        level--;
        currentExp = 0;
        maxHP *= 1 - (10 / 100);
        maxEP *= 1 - (10 / 100);

        attackPower += attackDefaultPower * 1 - (10 / 100);

        expToLevelUp = Mathf.RoundToInt(expToLevelUp / 1.5f);

        EventBus.OnLevelUp?.Invoke(level);
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
        EventBus.OnAttackPowerChanged?.Invoke(attackPower);
    }
}
