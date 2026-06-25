using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpOptionButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button actionButton;

    private TomeData assignedTome;

    public void Setup(TomeData tome)
    {
        assignedTome = tome;

        if (nameText != null)
        {
            nameText.text = tome.tomeName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = tome.description;
        }

        if (iconImage != null && tome.icon != null)
        {
            iconImage.sprite = tome.icon;
        }

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (GameplayUIManager.Instance != null && assignedTome != null)
        {
            GameplayUIManager.Instance.OnTomeSelected(assignedTome);
        }
    }
}