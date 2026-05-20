using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;

    public float CurrentHealth { get; private set; }
    public float CurrentShield { get; private set; }

    private float shieldRegenTimer;

    // Eventy, dzięki którym UI dowie się o zmianach bez ciągłego sprawdzania w Update
    public event Action OnHealthChanged;
    public event Action OnShieldChanged;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError($"[PlayerHealth] Brak komponentu PlayerStats na {gameObject.name}!");
            return;
        }

        // Ustawiamy startowe wartości na podstawie statystyk postaci
        CurrentHealth = stats.CurrentMaxHealth;
        CurrentShield = stats.CurrentMaxShield;

        // Wywołujemy eventy na start, żeby UI się zainicjalizowało
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

        // Wzór na pancerz: mnożnik obrażeń = 100 / (100 + pancerz)
        // Przy 0 pancerza: 100/100 = 1 (100% obrażeń)
        // Przy 100 pancerza: 100/200 = 0.5 (50% obrażeń)
        // Przy 300 pancerza: 100/400 = 0.25 (25% obrażeń)
        float damageMultiplier = 100f / (100f + stats.CurrentArmor);
        float finalDamage = rawDamage * damageMultiplier;

        // 1. Najpierw obrażenia przyjmuje Tarcza
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

        // 2. Reszta obrażeń idzie w HP
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

        // Regeneracja HP co sekundę
        if (CurrentHealth < stats.CurrentMaxHealth)
        {
            CurrentHealth += stats.CurrentHealthRegen * Time.deltaTime;
            CurrentHealth = Mathf.Min(CurrentHealth, stats.CurrentMaxHealth);
            OnHealthChanged?.Invoke();
        }

        // Odnawianie tarczy: 5% co 5 sekund
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
        Debug.Log("Gracz zginął!");
        // Tutaj dodasz ekran Game Over, restart poziomu itp.
    }
}