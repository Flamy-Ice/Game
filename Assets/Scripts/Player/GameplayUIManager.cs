using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject playerVisuals;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI gameOverTimerText;
    [SerializeField] private TextMeshProUGUI gameOverCurrencyText;
    [SerializeField] private TextMeshProUGUI gameOverKillsText;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private float gameOverDelay = 1.0f;

    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isDeathSequenceActive = false;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += StartHandlePlayerDeathSequence;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= StartHandlePlayerDeathSequence;
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (isGameOver || isDeathSequenceActive) return;
        TogglePause();
    }

    public void TogglePause()
    {
        if (isGameOver || isDeathSequenceActive) return;
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void StartHandlePlayerDeathSequence()
    {
        StartCoroutine(HandlePlayerDeathSequence());
    }

    private IEnumerator HandlePlayerDeathSequence()
    {
        isDeathSequenceActive = true;

        if (playerVisuals != null)
        {
            playerVisuals.SetActive(false);
        }

        yield return new WaitForSeconds(gameOverDelay);

        isGameOver = true;
        isDeathSequenceActive = false;

        UpdateGameOverSummary();

        hudCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    private void UpdateGameOverSummary()
    {
        if (GameTimer.Instance != null && gameOverTimerText != null)
        {
            gameOverTimerText.text = GameTimer.Instance.GetFormattedTime();
        }

        if (CurrencyManager.Instance != null && gameOverCurrencyText != null)
        {
            gameOverCurrencyText.text = CurrencyManager.Instance.TotalCurrency.ToString();
        }

        if (KillManager.Instance != null && gameOverKillsText != null)
        {
            gameOverKillsText.text = KillManager.Instance.TotalKills.ToString();
        }
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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