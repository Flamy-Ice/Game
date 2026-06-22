using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Image xpFillImage;
    public int baseXp = 100;
    public float xpExponent = 1.5f;
    public float smoothSpeed = 5f;

    private int currentLevel = 1;
    private int currentXp = 0;
    private int xpToNextLevel;
    private float targetFillAmount = 0f;

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
        CalculateXpRequired();
        if (xpFillImage != null)
        {
            xpFillImage.fillAmount = 0f;
        }
        targetFillAmount = 0f;
    }

    private void Update()
    {
        if (xpFillImage != null)
        {
            xpFillImage.fillAmount = Mathf.Lerp(xpFillImage.fillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
        }
    }

    public void AddXp(int amount)
    {
        currentXp += amount;

        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            currentLevel++;
            CalculateXpRequired();

            if (xpFillImage != null)
            {
                xpFillImage.fillAmount = 0f;
            }
        }

        targetFillAmount = (float)currentXp / xpToNextLevel;
    }

    private void CalculateXpRequired()
    {
        xpToNextLevel = Mathf.RoundToInt(baseXp * Mathf.Pow(currentLevel, xpExponent));
    }
}