using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    public GameObject damagePopupPrefab;
    public GameObject currencyPrefab;
    public GameObject expPrefab;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, bool isCrit)
    {
        currentHealth -= amount;

        if (damagePopupPrefab != null)
        {
            Vector3 spawnOffset = new Vector3(0, 1f, 0);

            if (Camera.main != null)
            {
                Vector3 directionToCamera = (Camera.main.transform.position - transform.position).normalized;
                spawnOffset += directionToCamera * 0.5f;
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