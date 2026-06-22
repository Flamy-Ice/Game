using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private EnemyStats enemyStats;
    private float nextAttackTime;

    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerHealth playerHealth = hit.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null && enemyStats != null)
                {
                    playerHealth.TakeDamage(enemyStats.Damage);
                    nextAttackTime = Time.time + enemyStats.AttackCooldown;
                }
            }
        }
    }
}