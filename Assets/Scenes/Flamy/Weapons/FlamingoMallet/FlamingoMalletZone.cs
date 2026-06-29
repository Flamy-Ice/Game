using UnityEngine;
using System.Collections;

public class FlamingoMalletZone : MonoBehaviour
{
    private int level = 1;
    private PlayerStats playerStats;
    private float baseRadius = 3.0f;
    private float currentRadius;

    private float baseDamage = 40f;
    private float damagePerLevel = 10f;
    private float lifeDuration = 1.5f;

    public void Setup(int weaponLevel, PlayerStats stats)
    {
        level = weaponLevel;
        playerStats = stats;
        currentRadius = baseRadius;

        if (level >= 10)
        {
            transform.localScale *= 2.0f;
            currentRadius *= 2.0f;
        }
        else if (level >= 5)
        {
            transform.localScale *= 1.5f;
            currentRadius *= 1.5f;
        }

        StartCoroutine(DelayedAttackRoutine());
        Destroy(gameObject, lifeDuration);
    }

    private IEnumerator DelayedAttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        float playerDmg = playerStats != null ? playerStats.Damage : 0f;
        float finalDamage = (baseDamage + (level - 1) * damagePerLevel) + playerDmg;

        bool isCrit = false;
        if (playerStats != null && Random.value <= playerStats.CritChance)
        {
            finalDamage *= playerStats.CritDamageMultiplier;
            isCrit = true;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(finalDamage, isCrit);
                }
            }
        }
    }
}