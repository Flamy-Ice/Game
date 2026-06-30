using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject levelUpCanvas;
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

    [Header("Level Up Settings")]
    [SerializeField] private GameObject levelUpOptionPrefab;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private TomeData[] allTomesPool;
    [SerializeField] private WeaponData[] allWeaponsPool;

    [Header("Chest Settings")]
    [SerializeField] private GameObject chestScreenCanvas;
    [SerializeField] private Transform chestOptionsContainer;
    [SerializeField] private GameObject chestAnimationPrefab;
    [SerializeField] private Transform chestAnimationContainer;
    [SerializeField] private GameObject chestOptionPrefab;

    [Header("Boss UI")]
    [SerializeField] private GameObject bossHealthBarContainer;
    [SerializeField] private Image bossHealthBarImage;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private float bossBarLerpSpeed = 5f;

    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isLevelUpActive = false;
    private bool isChestActive = false;
    private bool isDeathSequenceActive = false;
    private bool isMapChanging = false;
    private EnemyHealth activeBoss;
    private float bossTargetFill = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (bossHealthBarContainer != null)
        {
            bossHealthBarContainer.SetActive(false);
        }

        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(false);
        }

        if (chestScreenCanvas != null)
        {
            chestScreenCanvas.SetActive(false);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelUp += ShowLevelUpScreen;
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelUp -= ShowLevelUpScreen;
        }
    }

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

    private void Update()
    {
        if (bossHealthBarContainer != null && bossHealthBarContainer.activeSelf && bossHealthBarImage != null)
        {
            bossHealthBarImage.fillAmount = Mathf.Lerp(bossHealthBarImage.fillAmount, bossTargetFill, Time.unscaledDeltaTime * bossBarLerpSpeed);
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (isGameOver || isDeathSequenceActive || isLevelUpActive || isChestActive || isMapChanging) return;
        TogglePause();
    }

    public void TogglePause()
    {
        if (isGameOver || isDeathSequenceActive || isLevelUpActive || isChestActive || isMapChanging) return;
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void ShowLevelUpScreen()
    {
        if (isGameOver || isDeathSequenceActive || isMapChanging || isChestActive) return;

        PlayerStats playerStats = Object.FindFirstObjectByType<PlayerStats>();
        WeaponManager weaponManager = Object.FindFirstObjectByType<WeaponManager>();

        isLevelUpActive = true;
        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(true);
        }
        Time.timeScale = 0f;

        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        List<TomeData> availableTomes = new List<TomeData>();
        foreach (var tome in allTomesPool)
        {
            if (tome != null && playerStats != null && !playerStats.HasTome(tome))
            {
                availableTomes.Add(tome);
            }
        }

        List<WeaponData> availableWeapons = new List<WeaponData>();
        foreach (var weapon in allWeaponsPool)
        {
            if (weapon != null && weaponManager != null && weaponManager.GetWeaponLevel(weapon) < weapon.maxLevel)
            {
                availableWeapons.Add(weapon);
            }
        }

        int totalAvailable = availableTomes.Count + availableWeapons.Count;
        int optionsCount = Mathf.Min(3, totalAvailable);

        for (int i = 0; i < optionsCount; i++)
        {
            int randomIndex = Random.Range(0, availableTomes.Count + availableWeapons.Count);
            GameObject optionGO = Instantiate(levelUpOptionPrefab, optionsContainer);
            LevelUpOptionButton optionButton = optionGO.GetComponent<LevelUpOptionButton>();

            if (optionGO != null)
            {
                RectTransform rectTransform = optionGO.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = Vector2.zero;
                }
            }

            if (randomIndex < availableTomes.Count)
            {
                TomeData selectedTome = availableTomes[randomIndex];
                availableTomes.RemoveAt(randomIndex);
                if (optionButton != null)
                {
                    optionButton.Setup(selectedTome);
                }
            }
            else
            {
                int weaponIndex = randomIndex - availableTomes.Count;
                WeaponData selectedWeapon = availableWeapons[weaponIndex];
                availableWeapons.RemoveAt(weaponIndex);
                if (optionButton != null)
                {
                    optionButton.Setup(selectedWeapon, weaponManager.GetWeaponLevel(selectedWeapon));
                }
            }
        }

        if (optionsCount == 0)
        {
            if (playerHealth != null)
            {
                playerHealth.FullRestore();
            }
            CloseLevelUpScreen();
        }
    }

    public void ShowChestScreen()
    {
        if (isGameOver || isDeathSequenceActive || isMapChanging || isLevelUpActive || isChestActive) return;

        isChestActive = true;
        if (hudCanvas != null)
        {
            hudCanvas.SetActive(false);
        }
        if (chestScreenCanvas != null)
        {
            chestScreenCanvas.SetActive(true);
        }
        Time.timeScale = 0f;

        foreach (Transform child in chestOptionsContainer)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(ChestSequenceRoutine());
    }

    private IEnumerator ChestSequenceRoutine()
    {
        GameObject spawnedAnim = null;
        if (chestAnimationPrefab != null && chestAnimationContainer != null)
        {
            spawnedAnim = Instantiate(chestAnimationPrefab, chestAnimationContainer);
            RectTransform animRect = spawnedAnim.GetComponent<RectTransform>();
            if (animRect != null)
            {
                animRect.anchoredPosition = Vector2.zero;
            }
        }

        float duration = 2.0f;
        if (spawnedAnim != null)
        {
            Animator animator = spawnedAnim.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Update(0f);
                yield return null;
                duration = animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }

        yield return new WaitForSecondsRealtime(duration);

        if (spawnedAnim != null)
        {
            Destroy(spawnedAnim);
        }

        PlayerStats playerStats = Object.FindFirstObjectByType<PlayerStats>();
        WeaponManager weaponManager = Object.FindFirstObjectByType<WeaponManager>();

        List<TomeData> availableTomes = new List<TomeData>();
        foreach (var tome in allTomesPool)
        {
            if (tome != null && playerStats != null && !playerStats.HasTome(tome))
            {
                availableTomes.Add(tome);
            }
        }

        List<WeaponData> availableWeapons = new List<WeaponData>();
        foreach (var weapon in allWeaponsPool)
        {
            if (weapon != null && weaponManager != null && weaponManager.GetWeaponLevel(weapon) < weapon.maxLevel)
            {
                availableWeapons.Add(weapon);
            }
        }

        int totalAvailable = availableTomes.Count + availableWeapons.Count;
        if (totalAvailable > 0)
        {
            GameObject prefabToUse = chestOptionPrefab != null ? chestOptionPrefab : levelUpOptionPrefab;
            int randomIndex = Random.Range(0, totalAvailable);
            GameObject optionGO = Instantiate(prefabToUse, chestOptionsContainer);
            LevelUpOptionButton optionButton = optionGO.GetComponent<LevelUpOptionButton>();

            if (randomIndex < availableTomes.Count)
            {
                TomeData selectedTome = availableTomes[randomIndex];
                if (optionButton != null)
                {
                    optionButton.Setup(selectedTome);
                }
            }
            else
            {
                int weaponIndex = randomIndex - availableTomes.Count;
                WeaponData selectedWeapon = availableWeapons[weaponIndex];
                if (optionButton != null)
                {
                    optionButton.Setup(selectedWeapon, weaponManager.GetWeaponLevel(selectedWeapon));
                }
            }

            if (optionGO != null)
            {
                RectTransform buttonRect = optionGO.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    buttonRect.anchoredPosition = Vector2.zero;
                }
            }
        }
        else
        {
            CloseLevelUpScreen();
        }
    }

    public void OnTomeSelected(TomeData tome)
    {
        PlayerStats playerStats = Object.FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.AddTome(tome);
        }

        if (playerHealth != null)
        {
            playerHealth.FullRestore();
        }

        CloseLevelUpScreen();
    }

    public void OnWeaponSelected(WeaponData weapon)
    {
        WeaponManager weaponManager = Object.FindFirstObjectByType<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.AddOrUpgradeWeapon(weapon);
        }

        if (playerHealth != null)
        {
            playerHealth.FullRestore();
        }

        CloseLevelUpScreen();
    }

    public void CloseLevelUpScreen()
    {
        isLevelUpActive = false;
        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(false);
        }

        isChestActive = false;
        if (chestScreenCanvas != null)
        {
            chestScreenCanvas.SetActive(false);
        }

        if (!isPaused)
        {
            Time.timeScale = 1f;
        }

        if (hudCanvas != null && !isPaused && !isGameOver)
        {
            hudCanvas.SetActive(true);
        }
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

    public void RegisterBoss(EnemyHealth boss, string name)
    {
        StopAllCoroutines();
        activeBoss = boss;
        bossTargetFill = 1f;

        if (bossHealthBarImage != null)
        {
            bossHealthBarImage.fillAmount = 1f;
        }

        CanvasGroup canvasGroup = bossHealthBarContainer.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (bossNameText != null)
        {
            bossNameText.text = name;
        }

        if (bossHealthBarContainer != null)
        {
            bossHealthBarContainer.SetActive(true);
        }
    }

    public void UpdateBossHealth()
    {
        if (activeBoss != null)
        {
            float max = activeBoss.MaxHp;
            bossTargetFill = max > 0 ? activeBoss.CurrentHp / max : 0f;
        }
    }

    public void UnregisterBoss()
    {
        activeBoss = null;
        bossTargetFill = 0f;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOutBossUI());
        }
    }

    private IEnumerator FadeOutBossUI()
    {
        yield return new WaitForSeconds(0.4f);

        CanvasGroup canvasGroup = bossHealthBarContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = bossHealthBarContainer.AddComponent<CanvasGroup>();
        }

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        bossHealthBarContainer.SetActive(false);
        canvasGroup.alpha = 1f;
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

    public void SetMapChanging(bool state)
    {
        isMapChanging = state;
    }
}