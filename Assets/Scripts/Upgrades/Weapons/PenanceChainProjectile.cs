using UnityEngine;
using System.Collections.Generic;

public class PenanceChainProjectile : MonoBehaviour
{
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private float orbitRadius = 4.5f;
    [SerializeField] private float selfSpinSpeed = 360f;
    [SerializeField] private float hitRadius = 2.0f;

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
            Vector3 targetCenter = centerPoint.position + positionOffset;
            transform.position = targetCenter + new Vector3(0, 0, orbitRadius);
        }

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (centerPoint == null) return;

        age += Time.deltaTime;
        float currentAngle = (age / duration) * 360f;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * orbitRadius;

        transform.position = centerPoint.position + positionOffset + offset;

        transform.Rotate(0f, selfSpinSpeed * Time.deltaTime, 0f, Space.Self);

        Vector3 currentEuler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(0f, currentEuler.y, 0f);

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