using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamageTaken;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;
    public bool IsDead => currentHealth <= 0f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        float actualDamage = Mathf.Max(0f, damage);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0f, currentHealth);
        
        OnDamageTaken?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(currentHealth);

        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float healAmount)
    {
        if (IsDead) return;

        float actualHeal = Mathf.Max(0f, healAmount);
        currentHealth += actualHeal;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        
        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        float healthRatio = HealthPercentage;
        maxHealth = Mathf.Max(1f, newMaxHealth);
        currentHealth = maxHealth * healthRatio;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void Kill()
    {
        SetHealth(0f);
    }
}
