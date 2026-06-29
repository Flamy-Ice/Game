using UnityEngine;
using System.Collections.Generic;

public class ReaperScytheProjectile : MonoBehaviour
{
    [SerializeField] private float orbitRadius = 3.5f;
    [SerializeField] private float selfSpinSpeed = 720f;
    [SerializeField] private float hitRadius = 1.5f;

    private Transform centerPoint;
    private float damage;
    private bool isCrit;
    private float duration;
    private float age = 0f;

    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    public void Setup(Transform center, float damageValue, bool crit, float lifeTime)
    {
        centerPoint = center;
        damage = damageValue;
        isCrit = crit;
        duration = lifeTime;

        if (centerPoint != null)
        {
            transform.position = centerPoint.position + new Vector3(0, 0, orbitRadius);
        }
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (centerPoint == null) return;

        age += Time.deltaTime;
        float currentAngle = (age / duration) * 360f;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * orbitRadius;
        transform.position = centerPoint.position + offset;

        transform.Rotate(Vector3.up, selfSpinSpeed * Time.deltaTime, Space.Self);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null && !hitEnemies.Contains(enemyHealth))
                {
                    hitEnemies.Add(enemyHealth);
                    enemyHealth.TakeDamage(damage, isCrit);
                }
            }
        }
    }
}