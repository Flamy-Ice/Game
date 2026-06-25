using UnityEngine;

public enum TomeScalingType { Value, Percentage }
public enum TomeStatType
{
    MovementSpeed, JumpHeightMultiplier, ExtraJumps, MaxHp, HpRegen, Shield, Armor,
    Damage, AttackSpeedMultiplier, CritChance, CritDamageMultiplier, DodgeChance,
    Lifesteal, Thorns, ProjectileSizeMultiplier, ProjectileSpeedMultiplier, KnockbackMultiplier,
    DurationMultiplier, LuckMultiplier, PickupRangeMultiplier, XpGainMultiplier, CurrencyGainMultiplier
}

[CreateAssetMenu(fileName = "TomeData", menuName = "Stats/TomeData")]
public class TomeData : ScriptableObject
{
    public string tomeName;
    public string description;
    public TomeStatType statToModify;
    public float baseValue;
    public float scaleByLevel;
    public TomeScalingType scalingType;
    public bool isLimit;
    public int maxLevel;
}