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

    public void Setup(Transform targetEnemy, float damageAmount, bool isCrit)
    {
        target = targetEnemy;
        damage = damageAmount;
        isCritical = isCrit;
        if (target != null)
        {
            lastTargetPosition = target.position + Vector3.up * heightOffset;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            lastTargetPosition = target.position + Vector3.up * heightOffset;
        }

        Vector3 direction = (lastTargetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, lastTargetPosition) < 0.2f && target == null)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical);
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical);
            }
            Destroy(gameObject);
        }
    }
}