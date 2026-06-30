using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public GameObject damagePopupPrefab;
    public GameObject currencyPrefab;
    public GameObject expPrefab;

    [SerializeField] private float popupYOffset = 1f;
    [SerializeField] private float popupCameraOffset = 0.5f;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private string bossName = "Boss";

    private EnemyStats enemyStats;
    private float currentHealth;

    public float MaxHp => enemyStats != null ? enemyStats.MaxHp : 0f;
    public float CurrentHp => currentHealth;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.ScaleStatsToPlayerLevel();
            currentHealth = enemyStats.MaxHp;
        }

        if (isBoss && GameplayUIManager.Instance != null)
        {
            GameplayUIManager.Instance.RegisterBoss(this, bossName);
        }
    }

    public void TakeDamage(float amount, bool isCrit)
    {
        currentHealth -= amount;

        if (isBoss && GameplayUIManager.Instance != null)
        {
            GameplayUIManager.Instance.UpdateBossHealth();
        }

        if (damagePopupPrefab != null)
        {
            Vector3 spawnOffset = new Vector3(0, popupYOffset, 0);

            if (Camera.main != null)
            {
                Vector3 directionToCamera = (Camera.main.transform.position - transform.position).normalized;
                spawnOffset += directionToCamera * Mathf.Abs(popupCameraOffset);
            }

            GameObject popupGO = Instantiate(damagePopupPrefab, transform.position + spawnOffset, Quaternion.identity);
            DamagePopup popup = popupGO.GetComponent<DamagePopup>();
            if (popup != null)
            {
                popup.Setup(amount, isCrit);
            }
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isBoss && GameplayUIManager.Instance != null)
        {
            GameplayUIManager.Instance.UnregisterBoss();
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 0.2f;

        if (currencyPrefab != null)
        {
            Instantiate(currencyPrefab, spawnPosition, Quaternion.identity);
        }

        if (expPrefab != null)
        {
            Instantiate(expPrefab, spawnPosition, Quaternion.identity);
        }

        if (KillManager.Instance != null)
        {
            KillManager.Instance.AddKill();
        }

        Destroy(gameObject);
    }
}