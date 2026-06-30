using UnityEngine;

public class VorpalBlade : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject shaderVisualPrefab;
    [SerializeField] private float visualDuration = 1.0f;
    [SerializeField] private float forwardOffset = 1.5f;

    private int currentLevel = 1;
    private float cooldownTimer = 0f;
    private int attackCounter = 0;
    private PlayerStats playerStats;
    private Transform firePoint;

    private float baseDamage = 30f;
    private float damagePerLevel = 6f;
    private float baseCooldown = 0.8f;
    private float cooldownReductionPerLevel = 0.05f;
    private float baseRange = 4f;

    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();

        PlayerCombat combat = GetComponentInParent<PlayerCombat>();
        if (combat != null && combat.firePoint != null)
        {
            firePoint = combat.firePoint;
        }
        else
        {
            firePoint = transform.parent != null ? transform.parent.Find("FirePoint") : null;
            if (firePoint == null) firePoint = transform;
        }
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            float currentCooldown = baseCooldown - (currentLevel - 1) * cooldownReductionPerLevel;
            cooldownTimer = currentCooldown;
            ExecuteAttack();
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
    }

    private void ExecuteAttack()
    {
        float currentRange = baseRange;
        if (currentLevel >= 10)
        {
            currentRange *= 2f;
        }

        Transform target = FindClosestEnemy(currentRange);
        Quaternion spawnRotation = firePoint.rotation;

        if (target != null)
        {
            Vector3 direction = (target.position - firePoint.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                spawnRotation = Quaternion.LookRotation(direction);
            }
        }

        Vector3 attackDirection = spawnRotation * Vector3.forward;
        Vector3 spawnPosition = firePoint.position + (attackDirection * forwardOffset);

        if (shaderVisualPrefab != null)
        {
            GameObject visualInstance = Instantiate(shaderVisualPrefab, spawnPosition, spawnRotation, firePoint);
            Destroy(visualInstance, visualDuration);
        }

        attackCounter++;
        bool executeEffect = (currentLevel >= 5) && (attackCounter % 3 == 0);

        float weaponDamage = baseDamage + (currentLevel - 1) * damagePerLevel;
        float totalDamage = weaponDamage + (playerStats != null ? playerStats.Damage : 0f);

        bool isCrit = false;
        if (playerStats != null && Random.value <= playerStats.CritChance)
        {
            totalDamage *= playerStats.CritDamageMultiplier;
            isCrit = true;
        }

        Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, currentRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    if (executeEffect && (enemyHealth.CurrentHp / enemyHealth.MaxHp) < 0.25f)
                    {
                        enemyHealth.TakeDamage(enemyHealth.CurrentHp, false);
                    }
                    else
                    {
                        enemyHealth.TakeDamage(totalDamage, isCrit);
                    }
                }
            }
        }
    }

    private Transform FindClosestEnemy(float maxDistance)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = firePoint != null ? firePoint.position : transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < shortestDistance && distance <= maxDistance)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }
        return closest != null ? closest.transform : null;
    }
}