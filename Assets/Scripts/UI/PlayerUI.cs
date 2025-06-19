using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Slider hpBar;
    public Slider epBar;
    public Slider expBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moveSpeedText;

    private void Start()
    {
        EventBus.OnHpChanged += UpdateHP;
        EventBus.OnEpChanged += UpdateEP;
        EventBus.OnExpChanged += UpdateEXP;
        EventBus.OnLevelUp += UpdateLevel;
        EventBus.OnMoveSpeedChanged += UpdateMoveSpeed;
    }

    private void OnDestroy()
    {
        EventBus.OnHpChanged -= UpdateHP;
        EventBus.OnEpChanged -= UpdateEP;
        EventBus.OnExpChanged -= UpdateEXP;
        EventBus.OnLevelUp -= UpdateLevel;
        EventBus.OnMoveSpeedChanged -= UpdateMoveSpeed;
    }

    private void UpdateHP(int current, int max)
    {
        hpBar.maxValue = max;
        hpBar.value = current;
    }
    private void UpdateEP(int current, int max)
    {
        epBar.maxValue = max;
        epBar.value = current;
    }
    private void UpdateEXP(int current, int max)
    {
        expBar.maxValue = max;
        expBar.value = current;
    }
    private void UpdateLevel(int level)
    {
        levelText.SetText($"LV {level}");
    }
    private void UpdateMoveSpeed(float speed)
    {
        moveSpeedText.SetText($"{speed:F1}");
    }
}