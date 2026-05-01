using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "RPG/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float healthRegen = 0f; // Per second
    public float maxShield = 0f;
    public float armor = 0f;

    [Header("Percentages (0.0 to 1.0)")]
    [Range(0, 1)] public float dodgeChance = 0f;
    [Range(0, 1)] public float lifeSteal = 0f;
    [Range(0, 1)] public float thorns = 0f;
}