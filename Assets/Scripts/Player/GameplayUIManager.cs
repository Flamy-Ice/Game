using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private CanvasManager canvasManager;

    private bool isPaused = false;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ReturnToMainMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ReturnToMainMenuSingleScene()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuCanvas.SetActive(false);

        if (canvasManager == null)
        {
            canvasManager = Object.FindFirstObjectByType<CanvasManager>();
        }

        if (canvasManager != null)
        {
            canvasManager.ShowMainMenu();
        }
    }
}