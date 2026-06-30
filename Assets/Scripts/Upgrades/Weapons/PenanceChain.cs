using UnityEngine;

public class PenanceChain : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject chainPrefab;

    private int currentLevel = 1;
    private float cooldownTimer = 0f;
    private Transform firePoint;

    private float baseDamage = 35f;
    private float damagePercentPerLevel = 0.15f;
    private float baseCooldown = 1.2f;
    private float cooldownReductionPerLevel = 0.06f;

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
            currentCooldown = Mathf.Max(currentCooldown, 0.15f);
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
        if (chainPrefab == null) return;

        GameObject chainInstance = Instantiate(chainPrefab, firePoint.position, Quaternion.identity);
        PenanceChainProjectile chainScript = chainInstance.GetComponent<PenanceChainProjectile>();

        if (chainScript != null)
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

            chainScript.Setup(firePoint, totalDamage, isCrit, duration);
        }
    }
}