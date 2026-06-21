using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText;

    public void DisplayStats(CharacterStatsData data)
    {
        if (data == null || statsText == null) return;

        statsText.text =
            $"HP: {data.maxHp:F0}\n" +
            $"Shield: {data.shield:F0}\n" +
            $"Armor: {data.armor:F0}\n" +
            $"Speed: {data.movementSpeed:F1}\n" +
            $"Damage: {data.damage:F0}\n" +
            $"Attack Speed: {data.attackSpeedMultiplier:F2}x\n" +
            $"Crit Chance: {data.critChance * 100:F0}%\n" +
            $"Dodge: {data.dodgeChance * 100:F0}%\n" +
            $"Lifesteal: {data.lifesteal * 100:F0}%\n" +
            $"Luck: {data.luckMultiplier:F2}x";
    }
}