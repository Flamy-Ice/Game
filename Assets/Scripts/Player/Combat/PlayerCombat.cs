using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject[] projectilePrefabs;
    public Transform firePoint;
    public float range = 10f;

    private PlayerStats playerStats;
    private PlayerHealth playerHealth;
    private float fireCooldown = 0f;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        Transform target = FindClosestEnemy();

        if (target != null && fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / playerStats.AttackSpeedMultiplier;
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                closest = enemy;
            }
        }

        return closest != null ? closest.transform : null;
    }

    void Shoot(Transform target)
    {
        if (projectilePrefabs == null || projectilePrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, projectilePrefabs.Length);
        GameObject selectedPrefab = projectilePrefabs[randomIndex];

        if (selectedPrefab == null) return;

        Transform attackTarget = target;
        Transform customTargetPoint = target.Find("TargetPoint");

        if (customTargetPoint != null)
        {
            attackTarget = customTargetPoint;
        }

        GameObject projGO = Instantiate(selectedPrefab, firePoint.position, firePoint.rotation);

        if (playerStats != null)
        {
            projGO.transform.localScale *= playerStats.ProjectileSizeMultiplier;
        }

        Projectile projectile = projGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            float finalDamage = playerStats.Damage;
            bool isCrit = false;

            if (Random.value <= playerStats.CritChance)
            {
                finalDamage *= playerStats.CritDamageMultiplier;
                isCrit = true;
            }

            projectile.Setup(attackTarget, finalDamage, isCrit, playerHealth, playerStats.Lifesteal);
        }
    }
}