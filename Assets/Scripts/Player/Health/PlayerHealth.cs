using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private float damagePopupYOffset = 1.0f;

    private PlayerStats playerStats;
    private float hpRegenTimer;
    private float shieldRegenTimer;
    private bool isDead = false;

    public event Action OnHpChanged;
    public event Action OnShieldChanged;
    public event Action OnPlayerDeath;

    public float CurrentHp { get; private set; }
    public float MaxHp => playerStats != null ? playerStats.MaxHp : 0f;

    public float CurrentShield { get; private set; }
    public float MaxShield => playerStats != null ? playerStats.Shield : 0f;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        CurrentHp = MaxHp;
        CurrentShield = MaxShield;
        OnHpChanged?.Invoke();
        OnShieldChanged?.Invoke();
    }

    private void Update()
    {
        if (playerStats == null || isDead) return;

        if (CurrentHp < MaxHp)
        {
            shieldRegenTimer = 0f;
            hpRegenTimer += Time.deltaTime;

            if (hpRegenTimer >= 1f)
            {
                CurrentHp = Mathf.Clamp(CurrentHp + playerStats.HpRegen, 0f, MaxHp);
                OnHpChanged?.Invoke();
                hpRegenTimer -= 1f;
            }
        }
        else if (CurrentShield < MaxShield)
        {
            hpRegenTimer = 0f;
            shieldRegenTimer += Time.deltaTime;

            if (shieldRegenTimer >= 3f)
            {
                CurrentShield = Mathf.Clamp(CurrentShield + playerStats.HpRegen, 0f, MaxShield);
                OnShieldChanged?.Invoke();
                shieldRegenTimer -= 3f;
            }
        }
        else
        {
            hpRegenTimer = 0f;
            shieldRegenTimer = 0f;
        }
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, null);
    }

    public void TakeDamage(float amount, EnemyHealth attacker)
    {
        if (isDead) return;

        if (playerStats != null && UnityEngine.Random.value <= playerStats.DodgeChance)
        {
            SpawnDodgePopup();
            return;
        }

        if (playerStats != null)
        {
            amount *= 100f / (100f + (playerStats.Armor * 4f));
        }

        float totalDamageTaken = amount;

        if (attacker != null && playerStats != null && playerStats.Thorns > 0f)
        {
            float reflectedDamage = totalDamageTaken * playerStats.Thorns;
            if (reflectedDamage > 0f)
            {
                attacker.TakeDamage(reflectedDamage, false);
            }
        }

        if (CurrentShield > 0f)
        {
            float shieldDamage = Mathf.Min(amount, CurrentShield);
            CurrentShield -= shieldDamage;
            amount -= shieldDamage;
            OnShieldChanged?.Invoke();
        }

        if (amount > 0f)
        {
            CurrentHp = Mathf.Clamp(CurrentHp - amount, 0f, MaxHp);
            OnHpChanged?.Invoke();

            if (CurrentHp <= 0f)
            {
                isDead = true;
                OnPlayerDeath?.Invoke();
            }
        }

        if (totalDamageTaken > 0f)
        {
            SpawnDamagePopup(totalDamageTaken);
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f) return;

        CurrentHp = Mathf.Clamp(CurrentHp + amount, 0f, MaxHp);
        OnHpChanged?.Invoke();
    }

    private void SpawnDamagePopup(float damageAmount)
    {
        if (damagePopupPrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.up * damagePopupYOffset;
        GameObject popupGO = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
        DamagePopup popup = popupGO.GetComponent<DamagePopup>();

        if (popup != null)
        {
            popup.Setup(damageAmount, false, true, false);
        }
    }

    private void SpawnDodgePopup()
    {
        if (damagePopupPrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.up * damagePopupYOffset;
        GameObject popupGO = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
        DamagePopup popup = popupGO.GetComponent<DamagePopup>();

        if (popup != null)
        {
            popup.Setup(0f, false, false, true);
        }
    }
}