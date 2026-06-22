using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats playerStats;
    private float regenTimer;

    public event Action OnHpChanged;

    public float CurrentHp { get; private set; }
    public float MaxHp => playerStats != null ? playerStats.MaxHp : 0f;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        CurrentHp = MaxHp;
        OnHpChanged?.Invoke();
    }

    private void Update()
    {
        if (playerStats == null || CurrentHp >= MaxHp)
        {
            regenTimer = 0f;
            return;
        }

        regenTimer += Time.deltaTime;

        if (regenTimer >= 1f)
        {
            CurrentHp = Mathf.Clamp(CurrentHp + playerStats.HpRegen, 0f, MaxHp);
            OnHpChanged?.Invoke();
            regenTimer -= 1f;
        }
    }

    public void TakeDamage(float amount)
    {
        CurrentHp = Mathf.Clamp(CurrentHp - amount, 0f, MaxHp);
        OnHpChanged?.Invoke();
    }
}