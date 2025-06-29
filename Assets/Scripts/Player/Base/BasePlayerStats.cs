using UnityEngine;

public abstract class BasePlayerStats : MonoBehaviour, IPlayerStats
{
    [Header("Health & Energy")]
    public int maxHP = 100;
    protected int currentHP;
    public int maxEP = 50;
    protected int currentEP;
    
    [Header("Level System")]
    public int level = 1;
    protected int currentExp = 0;
    public int expToLevelUp = 100;
    
    [Header("Combat")]
    public int attackPower;
    protected readonly int attackDefaultPower = 10;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float epRecoveryRate = 5f;
    
    protected IPlayerMovement movement;
    protected IPlayerAttack attack;
    
    protected virtual void Awake()
    {
        movement = GetComponent<IPlayerMovement>();
        attack = GetComponent<IPlayerAttack>();
    }
    
    protected virtual void Start()
    {
        if (movement != null)
            movement.MoveSpeed = moveSpeed;
            
        attackPower = attackDefaultPower;
        currentHP = maxHP;
        currentEP = maxEP;
        
        InvokeStatsEvents();
    }
    
    protected virtual void InvokeStatsEvents()
    {
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
        EventBus.OnLevelUp?.Invoke(level);
        EventBus.OnAttackPowerChanged?.Invoke(attackPower);
        EventBus.OnMoveSpeedChanged?.Invoke(moveSpeed);
    }
    
    public virtual void TakeDamage(int damage)
    {
        if (movement?.IsInvincible == true) return;
        
        currentHP -= damage;
        if (currentHP <= 0) currentHP = 0;
        
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
    }
    
    public virtual void UseEP(int amount)
    {
        if (currentEP - amount < 0) return;
        currentEP -= amount;
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
    }
    
    public virtual void RecoverEP(int amount)
    {
        currentEP += amount;
        if (currentEP > maxEP) currentEP = maxEP;
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
    }
    
    public virtual void GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevelUp) LevelUp();
        EventBus.OnExpChanged?.Invoke(currentExp, expToLevelUp);
    }
    
    public virtual void AddMaxHP(int hp)
    {
        currentHP += hp;
        maxHP += hp;
        EventBus.OnHpChanged?.Invoke(currentHP, maxHP);
    }
    
    public virtual void AddMaxEP(int ep)
    {
        // 버그 수정: EP 값을 변경해야 함
        currentEP += ep;
        maxEP += ep;
        EventBus.OnEpChanged?.Invoke(currentEP, maxEP);
    }
    
    public virtual void AddAttackPower(int power)
    {
        attackPower += power;
        EventBus.OnAttackPowerChanged?.Invoke(attackPower);
    }
    
    public virtual void AddSpeed(float speed)
    {
        moveSpeed += speed;
        if (movement != null)
            movement.MoveSpeed = moveSpeed;
        EventBus.OnMoveSpeedChanged?.Invoke(moveSpeed);
    }
    
    public virtual void LevelUp()
    {
        level++;
        currentExp = 0;
        
        // 버그 수정: 정수 나눗셈 문제 해결
        maxHP = Mathf.RoundToInt(maxHP * 1.1f);  // 10% 증가
        maxEP = Mathf.RoundToInt(maxEP * 1.1f);  // 10% 증가
        
        currentHP = maxHP;
        currentEP = maxEP;
        
        attackPower = Mathf.RoundToInt(attackPower * 1.1f);  // 10% 증가
        expToLevelUp = Mathf.RoundToInt(expToLevelUp * 1.5f);
        
        InvokeStatsEvents();
    }
    
    public virtual void LevelDown()
    {
        if (level <= 1) return;
        
        level--;
        currentExp = 0;
        
        maxHP = Mathf.RoundToInt(maxHP / 1.1f);
        maxEP = Mathf.RoundToInt(maxEP / 1.1f);
        
        attackPower = Mathf.RoundToInt(attackPower / 1.1f);
        expToLevelUp = Mathf.RoundToInt(expToLevelUp / 1.5f);
        
        InvokeStatsEvents();
    }
}
