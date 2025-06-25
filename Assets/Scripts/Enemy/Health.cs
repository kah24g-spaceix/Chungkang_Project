using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            DamageDealer damage = other.GetComponent<DamageDealer>();
            TakeDamage(damage.damageAmount);
        }
    }
}
