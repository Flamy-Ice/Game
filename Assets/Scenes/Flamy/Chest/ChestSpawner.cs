using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private GameObject chestPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 30.0f;
    [SerializeField] private float minSpawnDistance = 12f;
    [SerializeField] private float maxSpawnDistance = 22f;

    private float nextSpawnTime;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnChest();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnChest()
    {
        if (chestPrefab == null) return;

        Vector3 spawnPosition = CalculateValidTerrainPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(chestPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 CalculateValidTerrainPosition()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle) * randomDistance, 0f, Mathf.Sin(randomAngle) * randomDistance);
        Vector3 targetWorldPosition = playerTransform.position + spawnOffset;

        float raycastHeight = targetWorldPosition.y + 25f;
        Vector3 raycastOrigin = new Vector3(targetWorldPosition.x, raycastHeight, targetWorldPosition.z);

        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hitInfo, 50f, terrainLayer))
        {
            return hitInfo.point + new Vector3(0f, 0.05f, 0f);
        }

        return Vector3.zero;
    }
}