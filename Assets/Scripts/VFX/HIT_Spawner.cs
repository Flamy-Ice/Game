using UnityEngine;
using UnityEngine.VFX;

public class Hit_Spawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject hitVFXPrefab; // Drag your VFX Prefab here
    public float spawnInterval = 1.0f; // Seconds between spawns
    public float destroyAfter = 2.0f;  // Seconds before the spawned object is deleted

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEffect();
            timer = 0;
        }
    }

    void SpawnEffect()
    {
        if (hitVFXPrefab != null)
        {
            // Create the VFX at the current object's position and rotation
            GameObject vfxInstance = Instantiate(hitVFXPrefab, transform.position, transform.rotation);

            // Clean up the object after a few seconds so the scene stays performant
            Destroy(vfxInstance, destroyAfter);
        }
    }
}