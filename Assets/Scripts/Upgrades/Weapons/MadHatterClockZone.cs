using UnityEngine;

public class MadHatterClockZone : MonoBehaviour
{
    private int level = 1;
    private PlayerStats playerStats;
    private float timer = 0f;
    private float tickInterval = 1f;
    private float zoneRadius = 3f;

    private float baseInitialDamage = 10f;
    private float initialDamagePerLevel = 1.5f;
    private float baseTickDamage = 2f;
    private float tickDamagePerLevel = 0.3f;

    public void Setup(int weaponLevel, PlayerStats stats)
    {
        level = weaponLevel;
        playerStats = stats;

        if (level >= 5)
        {
            transform.localScale *= 1.4f;
            zoneRadius *= 1.4f;
        }

        Destroy(gameObject, 5f);
        DealDamage(true);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        ApplySlowToEnemies();

        if (timer >= tickInterval)
        {
            timer -= tickInterval;
            DealDamage(false);
        }
    }

    private void DealDamage(bool isInitial)
    {
        float playerDmg = playerStats != null ? playerStats.Damage : 0f;
        float finalDamage = 0f;

        if (isInitial)
        {
            finalDamage = (baseInitialDamage + (level - 1) * initialDamagePerLevel) + playerDmg;
        }
        else
        {
            finalDamage = (baseTickDamage + (level - 1) * tickDamagePerLevel) + playerDmg;
        }

        if (level >= 10)
        {
            finalDamage *= 2f;
        }

        bool isCrit = false;
        if (playerStats != null && Random.value <= playerStats.CritChance)
        {
            finalDamage *= playerStats.CritDamageMultiplier;
            isCrit = true;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, zoneRadius);
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

    private void ApplySlowToEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, zoneRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                hitCollider.gameObject.SendMessage("ApplySlow", 0.5f, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}