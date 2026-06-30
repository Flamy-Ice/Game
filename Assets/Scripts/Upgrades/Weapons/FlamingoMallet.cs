using UnityEngine;

public class FlamingoMallet : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject malletZonePrefab;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float forwardOffset = 2.0f;

    private int currentLevel = 1;
    private float cooldownTimer = 0f;
    private Transform firePoint;

    private void Start()
    {
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
            cooldownTimer = attackCooldown;
            ExecuteAttack();
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
    }

    private void ExecuteAttack()
    {
        if (malletZonePrefab == null) return;

        Transform target = FindClosestEnemy();
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

        GameObject zoneInstance = Instantiate(malletZonePrefab, spawnPosition, spawnRotation, firePoint);
        FlamingoMalletZone zoneScript = zoneInstance.GetComponent<FlamingoMalletZone>();
        if (zoneScript != null)
        {
            PlayerStats playerStats = GetComponentInParent<PlayerStats>();
            zoneScript.Setup(currentLevel, playerStats);
        }
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = firePoint != null ? firePoint.position : transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }
        return closest != null ? closest.transform : null;
    }
}