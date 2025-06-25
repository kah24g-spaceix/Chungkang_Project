using UnityEngine;

public class AndroidAttackComponent : MonoBehaviour
{
    [SerializeField] private GameObject weapon;
    private CapsuleCollider weaponCollider;
    private DamageDealer weaponDamage;
    private AndroidPlayerStats stats;

    private void Awake()
    {
        if (weaponCollider == null)
            weaponCollider = weapon.GetComponent<CapsuleCollider>();
        if (weaponDamage == null)
            weaponDamage = weapon.GetComponent<DamageDealer>();
        if (stats == null)
            stats = GetComponent<AndroidPlayerStats>();
    }
    public void Start()
    {
        weaponDamage.damageAmount = stats.attackPower;
        weaponCollider.enabled = false;
    }
    public void Attack()
    {
        weaponDamage.damageAmount = stats.attackPower;
        weaponCollider.enabled = true;
    }

    public void AttackEnd()
    {
        weaponCollider.enabled = false;
    }
}