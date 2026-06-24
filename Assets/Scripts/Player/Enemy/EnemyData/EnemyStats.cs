using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyStatsData baseStats;

    public float MaxHp { get; private set; }
    public float WalkSpeed { get; private set; }
    public float Damage { get; private set; }
    public float AttackCooldown { get; private set; }

    private void Awake()
    {
        if (baseStats != null)
        {
            ApplyStats(baseStats);
        }
    }

    public void ApplyStats(EnemyStatsData data)
    {
        if (data == null) return;

        baseStats = data;

        MaxHp = data.maxHp;
        WalkSpeed = data.walkSpeed;
        Damage = data.damage;
        AttackCooldown = data.attackCooldown;
    }

    public void ScaleStatsToPlayerLevel()
    {
        if (baseStats == null || LevelManager.Instance == null) return;

        int playerLevel = LevelManager.Instance.CurrentLevel;

        MaxHp = baseStats.maxHp * (1f + (playerLevel - 1) * baseStats.hpGrowthPerLevel);
        Damage = baseStats.damage * (1f + (playerLevel - 1) * baseStats.damageGrowthPerLevel);
    }
}