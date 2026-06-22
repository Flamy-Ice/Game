using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float basePickupRange = 3f;
    public float attractionSpeed = 10f;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        float currentRadius = basePickupRange * playerStats.PickupRangeMultiplier;
        DroppedItem[] items = FindObjectsOfType<DroppedItem>();

        foreach (DroppedItem item in items)
        {
            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance <= currentRadius)
            {
                item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, attractionSpeed * Time.deltaTime);

                if (distance < 0.4f)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        float currentRadius = basePickupRange;

        if (playerStats != null)
        {
            currentRadius = basePickupRange * playerStats.PickupRangeMultiplier;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, currentRadius);
    }
}