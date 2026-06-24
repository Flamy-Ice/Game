using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
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
        if (isDead) return;

        if (playerStats != null)
        {
            amount *= 100f / (100f + (playerStats.Armor * 4f));
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
    }
}