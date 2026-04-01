using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform playerTransform;

    void Start()
    {
        // Find the object with the tag "Player" at the start
        GameObject player = GameObject.FindGameObjectWithTag("MainCamera");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("FacePlayer Script: No object with the tag 'Player' was found!");
        }
    }

    void Update()
    {
        // Safety check: only rotate if the player exists in the scene
        if (playerTransform != null)
        {
            // We create a target position that uses the Player's X and Z, 
            // but uses THIS object's Y position.
            // This prevents the object from tilting up or down.
            Vector3 targetPosition = new Vector3(
                playerTransform.position.x,
                transform.position.y,
                playerTransform.position.z
            );

            transform.LookAt(targetPosition);
        }
    }
}