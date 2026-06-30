using UnityEngine;

public class MadHatterClock : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject clockZonePrefab;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource attackAudioSource;

    private int currentLevel = 1;
    private float cooldownTimer = 0f;
    private float attackCooldown = 5f;
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
        if (clockZonePrefab == null) return;

        if (attackAudioSource != null)
        {
            attackAudioSource.Play();
        }

        Transform target = FindClosestEnemy();
        Quaternion spawnRotation = Quaternion.identity;

        if (target != null)
        {
            Vector3 direction = (target.position - firePoint.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                spawnRotation = Quaternion.LookRotation(direction);
            }
        }

        GameObject zoneInstance = Instantiate(clockZonePrefab, firePoint.position, spawnRotation);
        MadHatterClockZone zoneScript = zoneInstance.GetComponent<MadHatterClockZone>();
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