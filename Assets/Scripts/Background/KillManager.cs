using UnityEngine;
using TMPro;

public class KillManager : MonoBehaviour
{
    public static KillManager Instance { get; private set; }

    public TextMeshProUGUI killText;
    private int totalKills = 0;

    public int TotalKills => totalKills;

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

    public void AddKill()
    {
        totalKills++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (killText != null)
        {
            killText.text = totalKills.ToString();
        }
    }
}