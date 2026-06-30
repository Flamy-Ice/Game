using UnityEngine;

public class ReaperScythe : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject scythePrefab;

    private int currentLevel = 1;
    private float cooldownTimer = 0f;
    private Transform firePoint;

    private float baseDamage = 25f;
    private float damagePercentPerLevel = 0.15f;
    private float baseCooldown = 2.0f;
    private float cooldownReductionPerLevel = 0.1f;

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
            float currentCooldown = baseCooldown - (currentLevel - 1) * cooldownReductionPerLevel;
            currentCooldown = Mathf.Max(currentCooldown, 0.2f);
            cooldownTimer = currentCooldown;
            ExecuteAttack(currentCooldown);
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
    }

    private void ExecuteAttack(float duration)
    {
        if (scythePrefab == null) return;

        GameObject scytheInstance = Instantiate(scythePrefab, firePoint.position, Quaternion.identity);
        ReaperScytheProjectile scytheScript = scytheInstance.GetComponent<ReaperScytheProjectile>();

        if (scytheScript != null)
        {
            PlayerStats playerStats = GetComponentInParent<PlayerStats>();
            float totalDamage = baseDamage + (baseDamage * (currentLevel - 1) * damagePercentPerLevel);
            if (playerStats != null) totalDamage += playerStats.Damage;

            bool isCrit = false;
            if (playerStats != null && Random.value <= playerStats.CritChance)
            {
                totalDamage *= playerStats.CritDamageMultiplier;
                isCrit = true;
            }

            scytheScript.Setup(firePoint, totalDamage, isCrit, duration);
        }
    }
}