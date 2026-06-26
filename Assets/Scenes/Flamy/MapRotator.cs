using UnityEngine;
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

    private int currentMapIndex = 0;
    private float timer = 0f;

    private void Start()
    {
        InitializeMaps();
        UpdateSkybox();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f;
            SwitchToNextMap();
        }
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