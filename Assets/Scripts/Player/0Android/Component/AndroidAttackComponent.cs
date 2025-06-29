using UnityEngine;
using System.Collections;

public class AndroidAttackComponent : MonoBehaviour, IPlayerAttack
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject heavyWeapon;
    
    [Header("Combo Settings")]
    [SerializeField] private float comboTimeWindow = 1.5f;
    [SerializeField] private float[] attackDurations = {0.8f, 0.9f, 1.2f};
    [SerializeField] private float[] attackDamageMultipliers = {1f, 1.2f, 1.5f};
    [SerializeField] private float movementReduction = 0.2f;
    
    // Components
    private CapsuleCollider weaponCollider;
    private CapsuleCollider heavyWeaponCollider;
    private DamageDealer weaponDamage;
    private DamageDealer heavyWeaponDamage;
    private BasePlayerStats stats;
    private AndroidAnimationComponent animComponent;
    private AndroidMovementComponent movementComponent;
    private WeaponManager weaponManager;
    
    // Combo System
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool canCombo = false;
    private bool comboQueued = false;
    private float comboTimer = 0f;
    private Coroutine currentAttackCoroutine;
    
    // Weapon System
    private WeaponItem currentWeaponItem;
    
    // Movement restriction
    private float originalMovementSpeed;
    private bool movementRestricted = false;
    
    public bool IsAttacking => isAttacking;
    public int CurrentComboIndex => currentComboIndex;
    public bool CanCombo => canCombo;
    
    private void Awake()
    {
        // Get weapon components
        weaponCollider = weapon.GetComponent<CapsuleCollider>();
        weaponDamage = weapon.GetComponent<DamageDealer>();
        
        if (heavyWeapon != null)
        {
            heavyWeaponCollider = heavyWeapon.GetComponent<CapsuleCollider>();
            heavyWeaponDamage = heavyWeapon.GetComponent<DamageDealer>();
        }
        
        // Get player components
        stats = GetComponent<BasePlayerStats>();
        animComponent = GetComponent<AndroidAnimationComponent>();
        movementComponent = GetComponent<AndroidMovementComponent>();
        weaponManager = GetComponent<WeaponManager>();
    }
    
    private void Start()
    {
        // Initialize weapon damage
        UpdateWeaponDamage();
        
        // Disable weapon colliders initially
        weaponCollider.enabled = false;
        if (heavyWeaponCollider != null)
            heavyWeaponCollider.enabled = false;
            
        // Store original movement speed
        if (movementComponent != null)
            originalMovementSpeed = movementComponent.GetMovementSpeed();
        
        // Subscribe to events (EventBus 이벤트 수정)
        EventBus.OnAttackPowerChanged += OnAttackPowerChanged;
        
        // WeaponManager 이벤트 구독 (널 체크 추가)
        if (weaponManager != null)
        {
            WeaponManager.OnWeaponChanged += OnWeaponChanged;
        }
    }
    
    private void OnDestroy()
    {
        // EventBus 이벤트 구독 해제
        EventBus.OnAttackPowerChanged -= OnAttackPowerChanged;
        
        // WeaponManager 이벤트 구독 해제 (널 체크 추가)
        if (weaponManager != null)
        {
            WeaponManager.OnWeaponChanged -= OnWeaponChanged;
        }
    }
    
    // EventBus 이벤트 핸들러 (올바른 시그니처)
    private void OnAttackPowerChanged(int newAttackPower)
    {
        UpdateWeaponDamage();
    }
    
    private void OnWeaponChanged(WeaponItem newWeapon)
    {
        currentWeaponItem = newWeapon;
        UpdateWeaponDamage();
    }
    
    public void SetCurrentWeapon(WeaponItem weaponItem)
    {
        currentWeaponItem = weaponItem;
        UpdateWeaponDamage();
    }
    
    private void UpdateWeaponDamage()
    {
        int baseDamage = stats?.attackPower ?? 10;
        
        if (currentWeaponItem != null)
        {
            baseDamage = currentWeaponItem.GetDamage();
        }
        
        if (weaponDamage != null)
            weaponDamage.damageAmount = baseDamage;
            
        if (heavyWeaponDamage != null)
            heavyWeaponDamage.damageAmount = Mathf.RoundToInt(baseDamage * 1.5f);
    }
    
    public void Attack()
    {
        bool isHeavy = weaponManager != null && weaponManager.IsHeavyWeaponEquipped;
        Attack(isHeavy);
    }
    
    public void Attack(bool heavy)
    {
        if (isAttacking)
        {
            if (canCombo && currentComboIndex < 2)
            {
                comboQueued = true;
                Debug.Log($"Combo queued! Next attack will be combo {currentComboIndex + 1}");
            }
            return;
        }
        
        bool actualHeavy = weaponManager != null && weaponManager.IsHeavyWeaponEquipped;
        StartAttack(actualHeavy);
    }
    
    private void StartAttack(bool isHeavy = false)
    {
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
        }
        
        currentAttackCoroutine = StartCoroutine(AttackSequence(isHeavy));
    }
    
    private IEnumerator AttackSequence(bool isHeavy)
    {
        isAttacking = true;
        canCombo = false;
        comboQueued = false;
        
        RestrictMovement(true);
        SetAttackAnimation(isHeavy);
        
        yield return new WaitForSeconds(0.1f);
        EnableWeapon(isHeavy);
        
        float attackDuration = attackDurations[currentComboIndex];
        float comboStartTime = attackDuration * 0.4f;
        
        yield return new WaitForSeconds(comboStartTime);
        
        canCombo = true;
        comboTimer = comboTimeWindow;
        
        yield return new WaitForSeconds(attackDuration - comboStartTime);
        
        DisableWeapon();
        
        if (comboQueued && currentComboIndex < 2)
        {
            currentComboIndex++;
            Debug.Log($"Executing combo attack {currentComboIndex + 1}");
            
            bool nextIsHeavy = weaponManager != null && weaponManager.IsHeavyWeaponEquipped;
            currentAttackCoroutine = StartCoroutine(AttackSequence(nextIsHeavy));
        }
        else
        {
            EndAttack();
        }
    }
    
    private void SetAttackAnimation(bool isHeavy)
    {
        if (animComponent == null) return;
        
        if (isHeavy)
        {
            animComponent.SetHeavyAttack(currentComboIndex);
        }
        else
        {
            animComponent.SetComboAttack(currentComboIndex);
        }
        
        Debug.Log($"Playing attack animation - Combo: {currentComboIndex + 1}, Heavy: {isHeavy}");
    }
    
    private void EnableWeapon(bool isHeavy)
    {
        if (isHeavy && heavyWeaponCollider != null)
        {
            heavyWeaponCollider.enabled = true;
            float damage = GetWeaponDamage() * attackDamageMultipliers[currentComboIndex];
            heavyWeaponDamage.damageAmount = Mathf.RoundToInt(damage);
            Debug.Log($"Heavy weapon enabled with damage: {heavyWeaponDamage.damageAmount}");
        }
        else
        {
            weaponCollider.enabled = true;
            float damage = GetWeaponDamage() * attackDamageMultipliers[currentComboIndex];
            weaponDamage.damageAmount = Mathf.RoundToInt(damage);
            Debug.Log($"Normal weapon enabled with damage: {weaponDamage.damageAmount}");
        }
    }
    
    private int GetWeaponDamage()
    {
        if (currentWeaponItem != null)
        {
            return currentWeaponItem.GetDamage();
        }
        
        return stats?.attackPower ?? 10;
    }
    
    private void DisableWeapon()
    {
        weaponCollider.enabled = false;
        if (heavyWeaponCollider != null)
            heavyWeaponCollider.enabled = false;
            
        weaponDamage?.ResetDamageTracking();
        heavyWeaponDamage?.ResetDamageTracking();
    }
    
    private void EndAttack()
    {
        isAttacking = false;
        canCombo = false;
        comboQueued = false;
        comboTimer = 0f;
        
        RestrictMovement(false);
        
        if (animComponent != null)
        {
            animComponent.SetAttack(false);
        }
        
        StartCoroutine(ResetComboAfterDelay());
        Debug.Log("Attack ended");
    }
    
    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        
        if (!isAttacking && !canCombo)
        {
            currentComboIndex = 0;
            Debug.Log("Combo reset to 0");
        }
    }
    
    private void RestrictMovement(bool restrict)
    {
        if (movementComponent == null) return;
        
        if (restrict && !movementRestricted)
        {
            float restrictedSpeed = originalMovementSpeed * movementReduction;
            movementComponent.SetMovementSpeed(restrictedSpeed);
            movementRestricted = true;
        }
        else if (!restrict && movementRestricted)
        {
            movementComponent.SetMovementSpeed(originalMovementSpeed);
            movementRestricted = false;
        }
    }
    
    private void Update()
    {
        UpdateComboTimer();
    }
    
    private void UpdateComboTimer()
    {
        if (canCombo && comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            
            if (comboTimer <= 0f)
            {
                EndCombo();
            }
        }
    }
    
    private void EndCombo()
    {
        canCombo = false;
        comboTimer = 0f;
        comboQueued = false;
        
        if (!isAttacking)
        {
            StartCoroutine(ResetComboAfterDelay());
        }
    }
    
    public void ForceEndAttack()
    {
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
        }
        
        DisableWeapon();
        EndAttack();
    }
    
    // TryComboAttack 메서드 수정 (인수 제거)
    public bool TryComboAttack()
    {
        if (!canCombo || comboQueued) return false;
        
        comboQueued = true;
        return true;
    }
    
    // Legacy method for compatibility
    public void AttackEnd()
    {
        Debug.Log("AttackEnd called (legacy method)");
    }
}
