using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Characters/Enemy Stats")]
public class EnemyStatsData : ScriptableObject
{
    [Header("Stats")]
    public float moveSpeed = 4f;
    public float maxHealth = 50f;
    public float damage = 10f;
    public float attackSpeed = 1.5f;
}