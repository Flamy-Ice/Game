using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapRotator : MonoBehaviour
{
    [SerializeField] private List<GameObject> maps;
    [SerializeField] private float changeInterval = 30f;
    [SerializeField] private Transform player;
    [SerializeField] private float playerHeightOffset = 0f;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float enemyHeightOffset = 0f;
    [SerializeField] private string orbTag = "Orb";
    [SerializeField] private float orbHeightOffset = 0f;
    [SerializeField] private List<Material> skyboxes;

    [Header("UI Transition Settings")]
    [SerializeField] private GameObject mapChangeCanvas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private GameObject animationPrefab1;
    [SerializeField] private GameObject animationPrefab2;
    [SerializeField] private Transform animationContainer;
    [SerializeField] private float defaultAnimationDuration = 2.0f;

    private int currentMapIndex = 0;
    private float timer = 0f;
    private bool isTransitioning = false;

    private void Start()
    {
        InitializeMaps();
        UpdateSkybox();

        if (mapChangeCanvas != null)
        {
            mapChangeCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (isTransitioning) return;

        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f;
            StartCoroutine(MapChangeSequence());
        }
    }

    private IEnumerator MapChangeSequence()
    {
        isTransitioning = true;

        if (GameplayUIManager.Instance != null)
        {
            GameplayUIManager.Instance.SetMapChanging(true);
        }

        if (mapChangeCanvas != null)
        {
            mapChangeCanvas.SetActive(true);
        }

        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = 0f;
            backgroundImage.color = c;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                backgroundImage.color = c;
                yield return null;
            }
            c.a = 1f;
            backgroundImage.color = c;
        }

        Time.timeScale = 0f;

        SwitchToNextMap();

        GameObject selectedPrefab = Random.Range(0, 2) == 0 ? animationPrefab1 : animationPrefab2;
        GameObject spawnedAnim = null;

        if (selectedPrefab != null && animationContainer != null)
        {
            spawnedAnim = Instantiate(selectedPrefab, animationContainer);
        }

        float currentAnimDuration = defaultAnimationDuration;
        if (spawnedAnim != null)
        {
            Animator animator = spawnedAnim.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Update(0f);
                currentAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }

        yield return new WaitForSecondsRealtime(currentAnimDuration);

        if (spawnedAnim != null)
        {
            Destroy(spawnedAnim);
        }

        Time.timeScale = 1f;

        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Clamp01(1f - (elapsed / fadeDuration));
                backgroundImage.color = c;
                yield return null;
            }
            c.a = 0f;
            backgroundImage.color = c;
        }

        if (mapChangeCanvas != null)
        {
            mapChangeCanvas.SetActive(false);
        }

        if (GameplayUIManager.Instance != null)
        {
            GameplayUIManager.Instance.SetMapChanging(false);
        }

        isTransitioning = false;
    }

    private void InitializeMaps()
    {
        if (maps == null || maps.Count == 0) return;

        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i] != null)
            {
                maps[i].SetActive(i == currentMapIndex);
            }
        }
    }

    private void SwitchToNextMap()
    {
        if (maps == null || maps.Count <= 1) return;

        if (maps[currentMapIndex] != null)
        {
            maps[currentMapIndex].SetActive(false);
        }

        currentMapIndex = (currentMapIndex + 1) % maps.Count;

        if (maps[currentMapIndex] != null)
        {
            maps[currentMapIndex].SetActive(true);
            AdjustPlayerPosition();
            AdjustEnemiesPositionAndModels();
            AdjustOrbsPosition();
            UpdateSkybox();
        }
    }

    private void AdjustPlayerPosition()
    {
        if (player == null) return;

        CharacterController characterController = player.GetComponent<CharacterController>();

        Vector3 rayOrigin = new Vector3(player.position.x, 500f, player.position.z);
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1000f))
        {
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            player.position = new Vector3(player.position.x, hit.point.y + playerHeightOffset, player.position.z);

            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
    }

    private void AdjustEnemiesPositionAndModels()
    {
        if (string.IsNullOrEmpty(enemyTag)) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            CharacterController characterController = enemy.GetComponent<CharacterController>();
            Vector3 rayOrigin = new Vector3(enemy.transform.position.x, 500f, enemy.transform.position.z);
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1000f))
            {
                if (characterController != null)
                {
                    characterController.enabled = false;
                }

                enemy.transform.position = new Vector3(enemy.transform.position.x, hit.point.y + enemyHeightOffset, enemy.transform.position.z);

                if (characterController != null)
                {
                    characterController.enabled = true;
                }
            }

            EnemyModelSwitcher modelSwitcher = enemy.GetComponent<EnemyModelSwitcher>();
            if (modelSwitcher != null)
            {
                modelSwitcher.SwitchModel(currentMapIndex);
            }
        }
    }

    private void AdjustOrbsPosition()
    {
        if (string.IsNullOrEmpty(orbTag)) return;

        GameObject[] orbs = GameObject.FindGameObjectsWithTag(orbTag);

        foreach (GameObject orb in orbs)
        {
            if (orb == null) continue;

            CharacterController characterController = orb.GetComponent<CharacterController>();
            Vector3 rayOrigin = new Vector3(orb.transform.position.x, 500f, orb.transform.position.z);
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1000f))
            {
                if (characterController != null)
                {
                    characterController.enabled = false;
                }

                orb.transform.position = new Vector3(orb.transform.position.x, hit.point.y + orbHeightOffset, orb.transform.position.z);

                if (characterController != null)
                {
                    characterController.enabled = true;
                }
            }
        }
    }

    private void UpdateSkybox()
    {
        if (skyboxes == null || skyboxes.Count == 0) return;

        int skyboxIndex = currentMapIndex % skyboxes.Count;

        if (skyboxes[skyboxIndex] != null)
        {
            RenderSettings.skybox = skyboxes[skyboxIndex];
        }
    }
}