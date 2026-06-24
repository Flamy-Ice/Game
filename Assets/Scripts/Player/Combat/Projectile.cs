using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;
    public float heightOffset = 1f;
    public float baseKnockbackForce = 10f;

    private float damage;
    private bool isCritical;
    private Transform target;
    private Vector3 lastTargetPosition;
    private PlayerHealth playerHealth;
    private float lifestealChance;
    private float knockbackMultiplier;

    public void Setup(Transform targetEnemy, float damageAmount, bool isCrit, PlayerHealth playerHP, float lifesteal, float speedMultiplier, float knockbackMult)
    {
        target = targetEnemy;
        damage = damageAmount;
        isCritical = isCrit;
        playerHealth = playerHP;
        lifestealChance = lifesteal;
        knockbackMultiplier = knockbackMult;
        speed *= speedMultiplier;
        UpdateTargetPosition();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        UpdateTargetPosition();

        Vector3 direction = (lastTargetPosition - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, lastTargetPosition);

        if (distanceToTarget <= distanceThisFrame)
        {
            transform.position = lastTargetPosition;
            OnTargetReached(direction);
            return;
        }

        transform.position += direction * distanceThisFrame;
    }

    private void UpdateTargetPosition()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position;
            if (target.name != "TargetPoint")
            {
                targetPos += Vector3.up * heightOffset;
            }
            lastTargetPosition = targetPos;
        }
    }

    private void OnTargetReached(Vector3 hitDirection)
    {
        if (target != null)
        {
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();
            if (enemy == null)
            {
                enemy = target.GetComponentInParent<EnemyHealth>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical);
                ApplyLifesteal();
                ApplyKnockbackToEnemy(target.gameObject, hitDirection);
            }
        }
        Destroy(gameObject);
    }

    private void ProcessCollision(GameObject contactedObject)
    {
        EnemyHealth enemy = contactedObject.GetComponent<EnemyHealth>();
        if (enemy == null)
        {
            enemy = contactedObject.GetComponentInParent<EnemyHealth>();
        }

        if (enemy != null || contactedObject.CompareTag("Enemy"))
        {
            Vector3 hitDirection = (lastTargetPosition - transform.position).normalized;
            if (hitDirection == Vector3.zero) hitDirection = transform.forward;

            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical);
                ApplyLifesteal();
            }

            ApplyKnockbackToEnemy(contactedObject, hitDirection);
            Destroy(gameObject);
        }
    }

    private void ApplyLifesteal()
    {
        if (playerHealth != null && lifestealChance > 0f)
        {
            float healAmount = damage * lifestealChance;
            playerHealth.Heal(healAmount);
        }
    }

    private void ApplyKnockbackToEnemy(GameObject enemyObject, Vector3 direction)
    {
        EnemyMovement movement = enemyObject.GetComponent<EnemyMovement>();
        if (movement == null)
        {
            movement = enemyObject.GetComponentInParent<EnemyMovement>();
        }

        if (movement != null)
        {
            movement.ApplyKnockback(direction, baseKnockbackForce * knockbackMultiplier);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other.gameObject);
    }
}