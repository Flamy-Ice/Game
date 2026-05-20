using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Characters/Character Stats")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Movement Stats")]
    public float moveSpeed = 10f;
    public float jumpHeightMultiplier = 1f;
    public int extraJumps = 1;

    [Header("Health & Defence Stats")]
    public float maxHealth = 100f;
    public float healthRegen = 0f;
    public float maxShield = 0f;
    public float armor = 0f;
}