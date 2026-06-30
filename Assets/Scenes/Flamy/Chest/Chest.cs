using UnityEngine;

public class Chest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameplayUIManager.Instance != null)
            {
                GameplayUIManager.Instance.ShowChestScreen();
            }
            Destroy(gameObject);
        }
    }
}