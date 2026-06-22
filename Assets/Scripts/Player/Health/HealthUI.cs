using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float lerpSpeed = 5f;

    private float targetFill = 1f;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHpChanged += UpdateHealthUI;
            UpdateHealthUI();
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHpChanged -= UpdateHealthUI;
        }
    }

    private void Update()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null) return;

        targetFill = playerHealth.MaxHp > 0 ? playerHealth.CurrentHp / playerHealth.MaxHp : 0f;

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(playerHealth.CurrentHp)} / {Mathf.CeilToInt(playerHealth.MaxHp)}";
        }
    }
}