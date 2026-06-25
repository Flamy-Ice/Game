using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask terrainLayer;

    [Header("Spawn Distance")]
    [SerializeField] private float minSpawnDistance = 16f;
    [SerializeField] private float maxSpawnDistance = 26f;

    [Header("Wave Configuration")]
    [SerializeField] private float spawnInterval = 5.0f;
    [SerializeField] private int baseWaveSize = 2;
    [SerializeField] private int maxWaveSize = 18;
    [SerializeField] private float waveGrowthRate = 0.05f;

    [Header("Basic Enemy Prefabs")]
    [SerializeField] private GameObject basicNormalPrefab;
    [SerializeField] private GameObject basicBuffedPrefab;

    [Header("Common Enemy Prefabs")]
    [SerializeField] private GameObject commonNormalPrefab;
    [SerializeField] private GameObject commonBuffedPrefab;

    [Header("Uncommon Enemy Prefabs")]
    [SerializeField] private GameObject uncommonNormalPrefab;
    [SerializeField] private GameObject uncommonBuffedPrefab;

    [Header("Boss Configuration")]
    [SerializeField] private GameObject[] bossPrefabs;
    [SerializeField] private float timeBetweenBosses = 120f;
    [SerializeField] private float bossCooldownAfterDeath = 30f;

    [Header("Base Variant Probabilities")]
    [SerializeField] private float buffedVariantChance = 10f;

    private float nextSpawnTime;
    private float nextBossSpawnTime;
    private GameObject activeBossInstance;
    private bool isBossCurrentlyActive;
    private bool isBossOnCooldown;
    private float bossCooldownTimer;
    private float gameSessionTimer;

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
        nextBossSpawnTime = timeBetweenBosses;
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        gameSessionTimer += Time.deltaTime;

        HandleRegularSpawning();
        HandleBossSpawning();
    }

    private void HandleRegularSpawning()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnWave();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void HandleBossSpawning()
    {
        if (isBossCurrentlyActive)
        {
            if (activeBossInstance == null)
            {
                isBossCurrentlyActive = false;
                isBossOnCooldown = true;
                bossCooldownTimer = bossCooldownAfterDeath;
            }
            return;
        }

        if (isBossOnCooldown)
        {
            bossCooldownTimer -= Time.deltaTime;
            if (bossCooldownTimer <= 0f)
            {
                isBossOnCooldown = false;
                nextBossSpawnTime = gameSessionTimer + timeBetweenBosses;
            }
            return;
        }

        if (gameSessionTimer >= nextBossSpawnTime)
        {
            SpawnBoss();
        }
    }

    private void SpawnWave()
    {
        int currentWaveSize = Mathf.Min(maxWaveSize, baseWaveSize + Mathf.FloorToInt(gameSessionTimer * waveGrowthRate));

        for (int i = 0; i < currentWaveSize; i++)
        {
            Vector3 spawnPosition = CalculateValidTerrainPosition();
            if (spawnPosition == Vector3.zero) continue;

            GameObject prefabToSpawn = DetermineEnemyTierAndVariant();
            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            }
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefabs == null || bossPrefabs.Length == 0) return;

        Vector3 spawnPosition = CalculateValidTerrainPosition();
        if (spawnPosition == Vector3.zero) return;

        int randomIndex = Random.Range(0, bossPrefabs.Length);
        GameObject selectedBossPrefab = bossPrefabs[randomIndex];

        if (selectedBossPrefab != null)
        {
            activeBossInstance = Instantiate(selectedBossPrefab, spawnPosition, Quaternion.identity);
            isBossCurrentlyActive = true;
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

    private GameObject DetermineEnemyTierAndVariant()
    {
        float basicChance = 0f;
        float commonChance = 0f;
        float uncommonChance = 0f;
        bool allowBuffed = false;

        if (gameSessionTimer < 30f)
        {
            basicChance = 100f;
            allowBuffed = false;
        }
        else if (gameSessionTimer >= 30f && gameSessionTimer < 60f)
        {
            basicChance = 100f;
            allowBuffed = true;
        }
        else if (gameSessionTimer >= 60f && gameSessionTimer < 120f)
        {
            basicChance = 60f;
            commonChance = 40f;
            allowBuffed = true;
        }
        else
        {
            basicChance = 45f;
            commonChance = 35f;
            uncommonChance = 20f;
            allowBuffed = true;
        }

        float totalWeight = basicChance + commonChance + uncommonChance;
        float rolledValue = Random.Range(0f, totalWeight);
        bool rollBuffed = allowBuffed && (Random.Range(0f, 100f) <= buffedVariantChance);

        if (rolledValue <= basicChance)
        {
            return rollBuffed ? basicBuffedPrefab : basicNormalPrefab;
        }
        else if (rolledValue <= basicChance + commonChance)
        {
            return rollBuffed ? commonBuffedPrefab : commonNormalPrefab;
        }
        else
        {
            return rollBuffed ? uncommonBuffedPrefab : uncommonNormalPrefab;
        }
    }
}