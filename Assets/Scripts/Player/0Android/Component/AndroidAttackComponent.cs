using UnityEngine;
using System.Collections;

public class AndroidAttackComponent : MonoBehaviour, IPlayerAttack
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject heavyWeapon;

    [Header("Attack Movement")]
    [SerializeField] private float attackMoveDistance = 4f; // 공격 시 앞으로 이동 거리
    [SerializeField] private float attackMoveSpeed = 5f; // 공격 이동 속도
    
    [Header("Combo Settings")]
    [SerializeField] private float comboTimeWindow = 1.5f;
    [SerializeField] private float[] attackDurations = { 0.8f, 0.9f, 1.2f };
    [SerializeField] private float[] attackDamageMultipliers = { 1f, 1.2f, 1.5f };

    // Components
    private CapsuleCollider weaponCollider;
    private CapsuleCollider heavyWeaponCollider;
    private DamageDealer weaponDamage;
    private DamageDealer heavyWeaponDamage;
    private BasePlayerStats stats;
    private AndroidAnimationComponent animComponent;
    private AndroidMovementComponent movementComponent;
    private AndroidInputComponent inputComponent;
    private WeaponManager weaponManager;

    // Attack System
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool canCombo = false;
    private bool comboQueued = false;
    private float comboTimer = 0f;
    private Coroutine currentAttackCoroutine;
    private int currentAttackDirection = 0; // 현재 공격 방향

    // Weapon System
    private WeaponItem currentWeaponItem;

    public bool IsAttacking => isAttacking;
    public int CurrentComboIndex => currentComboIndex;
    public bool CanCombo => canCombo;

    private void Awake()
    {
        // 컴포넌트 참조
        stats = GetComponent<BasePlayerStats>();
        animComponent = GetComponent<AndroidAnimationComponent>();
        movementComponent = GetComponent<AndroidMovementComponent>();
        inputComponent = GetComponent<AndroidInputComponent>();
        weaponManager = GetComponent<WeaponManager>();
        
        InitializeWeapons();
    }
    
    private void InitializeWeapons()
    {
        if (weapon != null)
        {
            weaponCollider = weapon.GetComponent<CapsuleCollider>();
            weaponDamage = weapon.GetComponent<DamageDealer>();
            
            if (weaponCollider == null)
                Debug.LogError("BasicWeapon GameObject에 CapsuleCollider가 없습니다!");
            if (weaponDamage == null)
                Debug.LogError("BasicWeapon GameObject에 DamageDealer가 없습니다!");
        }
        else
        {
            Debug.LogError("AndroidAttackComponent에 weapon GameObject가 할당되지 않았습니다!");
        }
    }

    private void Start()
    {
        UpdateWeaponDamage();
        
        if (weaponCollider != null)
            weaponCollider.enabled = false;
        if (heavyWeaponCollider != null)
            heavyWeaponCollider.enabled = false;

        // 이벤트 구독
        EventBus.OnAttackPowerChanged += OnAttackPowerChanged;
        if (weaponManager != null)
        {
            WeaponManager.OnWeaponChanged += OnWeaponChanged;
        }
    }

    private void OnDestroy()
    {
        EventBus.OnAttackPowerChanged -= OnAttackPowerChanged;
        if (weaponManager != null)
        {
            WeaponManager.OnWeaponChanged -= OnWeaponChanged;
        }
    }

    private void OnAttackPowerChanged(int newAttackPower)
    {
        UpdateWeaponDamage();
    }

    private void OnWeaponChanged(WeaponItem newWeapon)
    {
        currentWeaponItem = newWeapon;
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

        // 공격 방향 설정 (마우스 방향 기준)
        if (inputComponent != null)
        {
            currentAttackDirection = inputComponent.GetDirectionIndex();
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

        // 공격 애니메이션 시작 (방향 포함)
        SetAttackAnimation(isHeavy);
        
        // 공격 방향으로 앞으로 이동 시작
        StartCoroutine(AttackMovement());

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
    
    // 공격 시 앞으로 이동하는 코루틴
    private IEnumerator AttackMovement()
    {
        float moveDuration = attackMoveDistance / attackMoveSpeed;
        float elapsed = 0f;
        
        while (elapsed < moveDuration && isAttacking)
        {
            if (movementComponent != null)
            {
                movementComponent.MoveForwardInAttack(attackMoveSpeed, Time.deltaTime);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void SetAttackAnimation(bool isHeavy)
    {
        if (animComponent == null) return;

        if (isHeavy)
        {
            animComponent.SetDirectionalHeavyAttack(currentComboIndex, currentAttackDirection);
        }
        else
        {
            animComponent.SetDirectionalComboAttack(currentComboIndex, currentAttackDirection);
        }

        Debug.Log($"Playing directional attack - Combo: {currentComboIndex + 1}, Direction: {currentAttackDirection}, Heavy: {isHeavy}");
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
        else if (weaponCollider != null)
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
        if (weaponCollider != null)
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

    public bool TryComboAttack()
    {
        if (!canCombo || comboQueued) return false;

        comboQueued = true;
        return true;
    }

    // 호환성 메서드들
    public void PerformAttack()
    {
        Attack();
    }

    public bool IsInAttackAnimation => isAttacking || canCombo;
    public bool CanMoveWhileAttacking => false; // 공격 중 이동 불가 (앞으로 이동은 자동)

    public void ResetAttackFlags()
    {
        if (!isAttacking)
        {
            canCombo = false;
            comboQueued = false;
            comboTimer = 0f;
        }
    }

    public void AttackEnd()
    {
        Debug.Log("AttackEnd called (legacy method)");
    }

    public void SetCurrentWeapon(WeaponItem weaponItem)
    {
        currentWeaponItem = weaponItem;
        UpdateWeaponDamage();
    }
}
