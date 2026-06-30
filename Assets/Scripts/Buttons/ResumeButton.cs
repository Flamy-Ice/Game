using UnityEngine;
using UnityEngine.UI;

public class ResumeButton : MonoBehaviour
{
    [SerializeField] private GameplayUIManager gameplayUIManager;

    private void Start()
    {
        if (gameplayUIManager == null)
        {
            gameplayUIManager = Object.FindFirstObjectByType<GameplayUIManager>();
        }

        GetComponent<Button>().onClick.AddListener(ResumeGame);
    }

    private void ResumeGame()
    {
        if (gameplayUIManager != null)
        {
            gameplayUIManager.TogglePause();
        }
    }
}