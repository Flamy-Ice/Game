using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "Stats/EnemyStatsData")]
public class EnemyStatsData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHp;
    public float walkSpeed;
    public float damage;
    public float attackCooldown;
}