using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;

    public float CurrentHealth { get; private set; }
    public float CurrentShield { get; private set; }

    private float shieldRegenTimer;

    public event Action OnHealthChanged;
    public event Action OnShieldChanged;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        if (stats == null) return;

        CurrentHealth = stats.CurrentMaxHealth;
        CurrentShield = stats.CurrentMaxShield;

        OnHealthChanged?.Invoke();
        OnShieldChanged?.Invoke();
    }

    private void Update()
    {
        HandleRegeneration();
    }

    public void TakeDamage(float rawDamage)
    {
        if (rawDamage <= 0 || CurrentHealth <= 0) return;

        float damageMultiplier = 100f / (100f + stats.CurrentArmor);
        float finalDamage = rawDamage * damageMultiplier;

        if (CurrentShield > 0)
        {
            if (finalDamage <= CurrentShield)
            {
                CurrentShield -= finalDamage;
                finalDamage = 0;
            }
            else
            {
                finalDamage -= CurrentShield;
                CurrentShield = 0;
            }
            OnShieldChanged?.Invoke();
        }

        if (finalDamage > 0)
        {
            CurrentHealth -= finalDamage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, stats.CurrentMaxHealth);
            OnHealthChanged?.Invoke();

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void HandleRegeneration()
    {
        if (CurrentHealth <= 0) return;

        if (CurrentHealth < stats.CurrentMaxHealth)
        {
            CurrentHealth += stats.CurrentHealthRegen * Time.deltaTime;
            CurrentHealth = Mathf.Min(CurrentHealth, stats.CurrentMaxHealth);
            OnHealthChanged?.Invoke();
        }

        if (stats.CurrentMaxShield > 0 && CurrentShield < stats.CurrentMaxShield)
        {
            shieldRegenTimer += Time.deltaTime;
            if (shieldRegenTimer >= 5f)
            {
                shieldRegenTimer = 0f;
                CurrentShield += stats.CurrentMaxShield * 0.05f;
                CurrentShield = Mathf.Min(CurrentShield, stats.CurrentMaxShield);
                OnShieldChanged?.Invoke();
            }
        }
        else
        {
            shieldRegenTimer = 0f;
        }
    }

    private void Die()
    {
    }
}