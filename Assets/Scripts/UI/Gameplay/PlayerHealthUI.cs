using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Połączenia")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStats playerStats;

    [Header("Elementy UI (Wymagany Image Type: Filled)")]
    [SerializeField] private Image hpFilledImage;
    [SerializeField] private Image shieldFilledImage;

    [Header("Ustawienia Płynności")]
    [Tooltip("Im większa wartość, tym szybciej pasek dogania realne HP.")]
    [SerializeField] private float smoothSpeed = 8f;

    // Zmienne przechowujące cel, do którego dążymy
    private float targetHpFill = 1f;
    private float targetShieldFill = 0f;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            // Podpinamy się pod eventy – ale teraz tylko wyliczamy cel, nie zmieniamy grafiki od razu
            playerHealth.OnHealthChanged += CacheTargetHP;
            playerHealth.OnShieldChanged += CacheTargetShield;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= CacheTargetHP;
            playerHealth.OnShieldChanged -= CacheTargetShield;
        }
    }

    private void Start()
    {
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerStats == null) playerStats = FindObjectOfType<PlayerStats>();

        // Na samym starcie gry ustawiamy pasek bez animacji (na sztywno),
        // żeby zdrowie nie ładowało się płynnie od zera przy włączeniu gry.
        if (playerHealth != null && playerStats != null)
        {
            CalculateInitialFills();
        }
    }

    private void Update()
    {
        // W każdej klatce płynnie przesuwamy fillAmount w stronę celu
        SmoothUpdateBars();
    }

    private void CalculateInitialFills()
    {
        if (playerStats.CurrentMaxHealth > 0)
        {
            targetHpFill = playerHealth.CurrentHealth / playerStats.CurrentMaxHealth;
            if (hpFilledImage != null) hpFilledImage.fillAmount = targetHpFill;
        }

        if (playerStats.CurrentMaxShield > 0)
        {
            targetShieldFill = playerHealth.CurrentShield / playerStats.CurrentMaxShield;
            if (shieldFilledImage != null) shieldFilledImage.fillAmount = targetShieldFill;
        }
    }

    private void SmoothUpdateBars()
    {
        // Mathf.Lerp(od_ilu, do_ilu, czas) płynnie wygładza przejście
        if (hpFilledImage != null)
        {
            hpFilledImage.fillAmount = Mathf.Lerp(hpFilledImage.fillAmount, targetHpFill, Time.deltaTime * smoothSpeed);
        }

        if (shieldFilledImage != null)
        {
            shieldFilledImage.fillAmount = Mathf.Lerp(shieldFilledImage.fillAmount, targetShieldFill, Time.deltaTime * smoothSpeed);
        }
    }

    private void CacheTargetHP()
    {
        if (playerStats != null && playerHealth != null && playerStats.CurrentMaxHealth > 0)
        {
            // Zmiana HP? Zapisujemy tylko ile pasek MA mieć docelowo
            targetHpFill = playerHealth.CurrentHealth / playerStats.CurrentMaxHealth;
        }
    }

    private void CacheTargetShield()
    {
        if (playerStats != null && playerHealth != null && playerStats.CurrentMaxShield > 0)
        {
            targetShieldFill = playerHealth.CurrentShield / playerStats.CurrentMaxShield;
        }
        else
        {
            targetShieldFill = 0f;
        }
    }
}