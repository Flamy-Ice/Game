using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    private enum BarType { Health, Shield }

    [SerializeField] private BarType barType;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float lerpSpeed = 5f;

    private float targetFill = 1f;

    private void Start()
    {
        if (playerHealth != null)
        {
            if (barType == BarType.Health)
            {
                playerHealth.OnHpChanged += UpdateUI;
            }
            else
            {
                playerHealth.OnShieldChanged += UpdateUI;
            }
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            if (barType == BarType.Health)
            {
                playerHealth.OnHpChanged -= UpdateUI;
            }
            else
            {
                playerHealth.OnShieldChanged -= UpdateUI;
            }
        }
    }

    private void Update()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
        }
    }

    private void UpdateUI()
    {
        if (playerHealth == null) return;

        float current = barType == BarType.Health ? playerHealth.CurrentHp : playerHealth.CurrentShield;
        float max = barType == BarType.Health ? playerHealth.MaxHp : playerHealth.MaxShield;

        targetFill = max > 0 ? current / max : 0f;

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}