using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon System")]
    [SerializeField] private Transform weaponSocket; // 무기가 장착될 위치
    [SerializeField] private WeaponItem defaultWeapon;
    
    // 현재 장착된 무기
    private WeaponItem currentWeapon;
    private GameObject currentWeaponInstance;
    private AndroidAttackComponent attackComponent;
    private BasePlayerInput playerInput;
    
    public WeaponItem CurrentWeapon => currentWeapon;
    public bool IsHeavyWeaponEquipped => currentWeapon != null && currentWeapon.isHeavyWeapon;
    
    // Events
    public static System.Action<WeaponItem> OnWeaponChanged;
    
    void Awake()
    {
        attackComponent = GetComponent<AndroidAttackComponent>();
        playerInput = GetComponent<BasePlayerInput>();
    }
    
    void Start()
    {
        // 기본 무기 장착
        if (defaultWeapon != null)
        {
            EquipWeapon(defaultWeapon);
        }
    }
    
    void Update()
    {
        // 입력 시스템에 강화공격 모드 알림
        if (playerInput != null)
        {
            playerInput.SetHeavyAttackMode(IsHeavyWeaponEquipped);
        }
    }
    
    public void EquipWeapon(WeaponItem weaponItem)
    {
        if (weaponItem == null) 
        {
            Debug.LogWarning("Trying to equip null weapon item!");
            return;
        }
        
        // 기존 무기 제거
        UnequipCurrentWeapon();
        
        // 새 무기 장착
        currentWeapon = weaponItem;
        
        // 무기 프리팹이 있으면 생성
        if (weaponItem.weaponPrefab != null && weaponSocket != null)
        {
            currentWeaponInstance = Instantiate(weaponItem.weaponPrefab, weaponSocket);
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            currentWeaponInstance.transform.localRotation = Quaternion.identity;
            
            Debug.Log($"Weapon model instantiated: {weaponItem.weaponPrefab.name}");
        }
        else
        {
            Debug.LogWarning($"No weapon prefab or socket found for {weaponItem.weaponName}");
        }
        
        // 공격 컴포넌트에 무기 정보 전달
        if (attackComponent != null)
        {
            attackComponent.SetCurrentWeapon(weaponItem);
        }
        
        // 이벤트 발생
        OnWeaponChanged?.Invoke(weaponItem);
        
        Debug.Log($"Equipped weapon: {weaponItem.weaponName} (Heavy: {weaponItem.isHeavyWeapon})");
    }
    
    public void UnequipCurrentWeapon()
    {
        if (currentWeaponInstance != null)
        {
            DestroyImmediate(currentWeaponInstance);
            currentWeaponInstance = null;
            Debug.Log("Previous weapon unequipped");
        }
        
        currentWeapon = null;
    }
    
    public void ToggleHeavyMode()
    {
        // 인벤토리에서 호출될 메서드
        if (currentWeapon != null)
        {
            Debug.Log($"Heavy mode: {IsHeavyWeaponEquipped}");
        }
    }
    
    // 무기 정보 조회 메서드들
    public int GetCurrentWeaponDamage()
    {
        return currentWeapon?.GetDamage() ?? 10;
    }
    
    public float GetCurrentWeaponRange()
    {
        return currentWeapon?.GetAttackRange() ?? 1f;
    }
    
    public string GetCurrentWeaponName()
    {
        return currentWeapon?.weaponName ?? "No Weapon";
    }
    
    // 디버그용 정보 표시
    void OnGUI()
    {
        if (currentWeapon != null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Current Weapon: {currentWeapon.weaponName}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Heavy Weapon: {IsHeavyWeaponEquipped}");
            GUI.Label(new Rect(10, 50, 300, 20), $"Damage: {GetCurrentWeaponDamage()}");
        }
    }
}
