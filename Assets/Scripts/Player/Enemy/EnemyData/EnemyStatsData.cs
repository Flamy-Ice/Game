using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "Stats/EnemyStatsData")]
public class EnemyStatsData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHp;
    public float walkSpeed;
    public float damage;
    public float attackCooldown;

    [Header("Scaling Stats (Per Player Level)")]
    public float hpGrowthPerLevel = 0.1f;
    public float damageGrowthPerLevel = 0.1f;
}