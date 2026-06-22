using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    private float damage;
    private Transform target;
    private Vector3 lastTargetPosition;

    public void Setup(Transform targetEnemy, float damageAmount)
    {
        target = targetEnemy;
        damage = damageAmount;
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    void Update()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
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
                enemy.TakeDamage(damage);
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
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}