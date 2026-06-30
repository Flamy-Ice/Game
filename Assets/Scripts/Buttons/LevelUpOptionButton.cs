using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpOptionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button buttonComponent;

    private TomeData assignedTome;
    private WeaponData assignedWeapon;

    public void Setup(TomeData tome)
    {
        assignedTome = tome;
        assignedWeapon = null;

        if (nameText != null) nameText.text = tome.tomeName;
        if (descriptionText != null) descriptionText.text = tome.description;
        if (iconImage != null) iconImage.sprite = tome.icon;

        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnSelected);
        }
    }

    public void Setup(WeaponData weapon, int currentLevel)
    {
        assignedWeapon = weapon;
        assignedTome = null;

        if (nameText != null) nameText.text = weapon.weaponName + (currentLevel > 0 ? " Lvl " + (currentLevel + 1) : " New!");
        if (descriptionText != null) descriptionText.text = weapon.description;
        if (iconImage != null) iconImage.sprite = weapon.icon;

        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnSelected);
        }
    }

    private void OnSelected()
    {
        if (assignedTome != null)
        {
            GameplayUIManager.Instance.OnTomeSelected(assignedTome);
        }
        else if (assignedWeapon != null)
        {
            GameplayUIManager.Instance.OnWeaponSelected(assignedWeapon);
        }
    }
}