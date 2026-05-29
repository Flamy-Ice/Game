using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyStatsData defaultStats;

    // Stats
    public float CurrentMaxHealth { get; private set; }
    public float CurrentDamage { get; private set; }
    public float CurrentAttackSpeed { get; private set; }
    public float CurrentMoveSpeed { get; private set; }

    // Currenbt Health Stat
    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        if (defaultStats != null)
        {
            InitializeStats();
        }
        else
        {
            Debug.LogError($"[EnemyStats] Missing 'Default Stats' asset on object: {gameObject.name}!");
        }
    }

    private void InitializeStats()
    {
        CurrentMaxHealth = defaultStats.maxHealth;
        CurrentDamage = defaultStats.damage;
        CurrentAttackSpeed = defaultStats.attackSpeed;
        CurrentMoveSpeed = defaultStats.moveSpeed;
    }
}