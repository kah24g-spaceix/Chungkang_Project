using UnityEngine;
using UnityEngine.UI;

public class SimpleInventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button normalWeaponButton;
    [SerializeField] private Button heavyWeaponButton;
    [SerializeField] private Text currentWeaponText;
    
    [Header("Weapon Items")]
    [SerializeField] private WeaponItem normalWeapon;
    [SerializeField] private WeaponItem heavyWeapon;
    
    private WeaponManager weaponManager;
    
    void Start()
    {
        // WeaponManager 찾기
        weaponManager = FindFirstObjectByType<WeaponManager>();
        
        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager not found in the scene!");
            return;
        }
        
        // 버튼 이벤트 연결
        if (normalWeaponButton != null)
        {
            normalWeaponButton.onClick.AddListener(() => EquipWeapon(normalWeapon));
        }
        else
        {
            Debug.LogWarning("Normal weapon button not assigned!");
        }
            
        if (heavyWeaponButton != null)
        {
            heavyWeaponButton.onClick.AddListener(() => EquipWeapon(heavyWeapon));
        }
        else
        {
            Debug.LogWarning("Heavy weapon button not assigned!");
        }
        
        // 무기 변경 이벤트 구독
        WeaponManager.OnWeaponChanged += UpdateUI;
        
        // 초기 UI 업데이트
        UpdateUI(weaponManager.CurrentWeapon);
    }
    
    private void EquipWeapon(WeaponItem weapon)
    {
        if (weaponManager != null && weapon != null)
        {
            weaponManager.EquipWeapon(weapon); // 이제 오류가 해결됨
            Debug.Log($"UI: Equipping weapon {weapon.weaponName}");
        }
        else
        {
            if (weaponManager == null)
                Debug.LogError("WeaponManager is null!");
            if (weapon == null)
                Debug.LogError("Weapon item is null!");
        }
    }
    
    private void UpdateUI(WeaponItem currentWeapon)
    {
        if (currentWeaponText != null && currentWeapon != null)
        {
            string weaponType = currentWeapon.isHeavyWeapon ? "[강화] " : "[일반] ";
            currentWeaponText.text = weaponType + currentWeapon.weaponName;
        }
        else if (currentWeaponText != null)
        {
            currentWeaponText.text = "무기 없음";
        }
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        WeaponManager.OnWeaponChanged -= UpdateUI;
    }
    
    // 테스트용 키보드 입력
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && normalWeapon != null)
        {
            EquipWeapon(normalWeapon);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && heavyWeapon != null)
        {
            EquipWeapon(heavyWeapon);
        }
    }
}
