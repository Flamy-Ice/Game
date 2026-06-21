using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsData", menuName = "Stats/CharacterStatsData")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Movement Stats")]
    public float movementSpeed;
    public float jumpHeightMultiplier = 1.0f;
    public int extraJumps = 1;

    [Header("Health & Defense")]
    public float maxHp;
    public float hpRegen;
    public float shield;
    public float armor;

    [Header("Offense")]
    public float damage;
    public float attackSpeedMultiplier = 1.0f;
    public float critChance;
    public float critDamageMultiplier = 1.5f;

    [Header("Special Defense")]
    public float dodgeChance;
    public float lifesteal;
    public float thorns;

    [Header("Projectiles")]
    public float projectileSizeMultiplier = 1.0f;
    public float projectileSpeedMultiplier = 1.0f;
    public float knockbackMultiplier = 1.0f;

    [Header("General")]
    public float durationMultiplier = 1.0f;
    public float luckMultiplier = 1.0f;

    [Header("Economy & Progression")]
    public float pickupRangeMultiplier = 1.0f;
    public float xpGainMultiplier = 1.0f;
    public float currencyGainMultiplier = 1.0f;
}