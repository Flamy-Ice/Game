using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;
    public float heightOffset = 1f;
    private float damage;
    private bool isCritical;
    private Transform target;
    private Vector3 lastTargetPosition;
    private PlayerHealth playerHealth;
    private float lifestealChance;

    public void Setup(Transform targetEnemy, float damageAmount, bool isCrit, PlayerHealth playerHP, float lifesteal, float speedMultiplier)
    {
        target = targetEnemy;
        damage = damageAmount;
        isCritical = isCrit;
        playerHealth = playerHP;
        lifestealChance = lifesteal;
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
            OnTargetReached();
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

    private void OnTargetReached()
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
            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical);
                ApplyLifesteal();
            }
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

    void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other.gameObject);
    }
}