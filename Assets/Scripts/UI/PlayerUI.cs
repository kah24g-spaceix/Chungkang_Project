using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("EP")]
    [SerializeField] private Slider epBar;
    [SerializeField] private TextMeshProUGUI epText;

    [Header("EXP")]
    [SerializeField] private Slider expBar;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;

    private void Start()
    {
        EventBus.OnHpChanged += UpdateHP;
        EventBus.OnEpChanged += UpdateEP;
        EventBus.OnExpChanged += UpdateEXP;
        EventBus.OnLevelUp += UpdateLevel;
        EventBus.OnAttackPowerChanged += UpdateAttackPower;
        EventBus.OnMoveSpeedChanged += UpdateMoveSpeed;
    }

    private void OnDestroy()
    {
        EventBus.OnHpChanged -= UpdateHP;
        EventBus.OnEpChanged -= UpdateEP;
        EventBus.OnExpChanged -= UpdateEXP;
        EventBus.OnLevelUp -= UpdateLevel;
        EventBus.OnAttackPowerChanged -= UpdateAttackPower;
        EventBus.OnMoveSpeedChanged -= UpdateMoveSpeed;
    }

    private void UpdateHP(int current, int max)
    {
        hpBar.maxValue = max;
        hpBar.value = current;
        hpText.SetText($"{current} / {max}");
    }
    private void UpdateEP(int current, int max)
    {
        epBar.maxValue = max;
        epBar.value = current;
        epText.SetText($"{current} / {max}");
    }
    private void UpdateEXP(int current, int max)
    {
        expBar.maxValue = max;
        expBar.value = current;
        expText.SetText($"{current} / {max}");
    }
    private void UpdateLevel(int level)
    {
        levelText.SetText($"LV {level}");
    }
    private void UpdateAttackPower(int power)
    {
        powerText.SetText($"AP: {power}");
    }
    private void UpdateMoveSpeed(float speed)
    {
        moveSpeedText.SetText($"S: {speed:F1}");
    }
}