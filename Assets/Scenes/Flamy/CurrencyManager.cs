using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public TextMeshProUGUI currencyText;
    private int totalCurrency = 0;

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
        UpdateUI();
    }

    public void AddCurrency(int amount)
    {
        totalCurrency += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currencyText != null)
        {
            currencyText.text = totalCurrency.ToString();
        }
    }
}