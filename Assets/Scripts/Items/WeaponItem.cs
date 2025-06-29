using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public string description;
    public Sprite icon;
    
    [Header("Weapon Stats")]
    public int baseDamage = 10;
    public float attackSpeed = 1f;
    public bool isHeavyWeapon = false; // 강화 무기 여부
    
    [Header("Visual")]
    public GameObject weaponPrefab; // 실제 무기 모델
    public RuntimeAnimatorController animatorController; // 무기별 애니메이션
    
    [Header("Heavy Weapon Effects")]
    [SerializeField] private float heavyDamageMultiplier = 1.5f;
    [SerializeField] private float heavyAttackRange = 1.2f;
    [SerializeField] private Color weaponTrailColor = Color.red;
    
    public int GetDamage()
    {
        return isHeavyWeapon ? Mathf.RoundToInt(baseDamage * heavyDamageMultiplier) : baseDamage;
    }
    
    public float GetAttackRange()
    {
        return isHeavyWeapon ? heavyAttackRange : 1f;
    }
}
