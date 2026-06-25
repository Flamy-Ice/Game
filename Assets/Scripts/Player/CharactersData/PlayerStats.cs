using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private CharacterStatsData baseStats;

    private List<TomeData> activeTomes = new List<TomeData>();

    public float MovementSpeed { get; private set; }
    public float JumpHeightMultiplier { get; private set; }
    public int ExtraJumps { get; private set; }

    public float MaxHp { get; private set; }
    public float HpRegen { get; private set; }
    public float Shield { get; private set; }
    public float Armor { get; private set; }

    public float Damage { get; private set; }
    public float AttackSpeedMultiplier { get; private set; }
    public float CritChance { get; private set; }
    public float CritDamageMultiplier { get; private set; }

    public float DodgeChance { get; private set; }
    public float Lifesteal { get; private set; }
    public float Thorns { get; private set; }

    public float ProjectileSizeMultiplier { get; private set; }
    public float ProjectileSpeedMultiplier { get; private set; }
    public float KnockbackMultiplier { get; private set; }

    public float DurationMultiplier { get; private set; }
    public float LuckMultiplier { get; private set; }

    public float PickupRangeMultiplier { get; private set; }
    public float XpGainMultiplier { get; private set; }
    public float CurrencyGainMultiplier { get; private set; }

    private void Awake()
    {
        if (CharacterTransfer.SelectedStats != null)
        {
            ApplyStats(CharacterTransfer.SelectedStats);
        }
        else if (baseStats != null)
        {
            ApplyStats(baseStats);
        }
    }

    private void OnEnable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelUp += RecalculateStats;
        }
    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelUp -= RecalculateStats;
        }
    }

    private void Start()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelUp -= RecalculateStats;
            LevelManager.Instance.OnLevelUp += RecalculateStats;
        }
    }

    public void ApplyStats(CharacterStatsData data)
    {
        if (data == null) return;
        baseStats = data;
        RecalculateStats();
    }

    public bool AddTome(TomeData tome)
    {
        if (tome == null || activeTomes.Contains(tome)) return false;
        activeTomes.Add(tome);
        RecalculateStats();
        return true;
    }

    public bool HasTome(TomeData tome)
    {
        return activeTomes.Contains(tome);
    }

    public void RecalculateStats()
    {
        if (baseStats == null) return;

        MovementSpeed = GetModifiedValue(baseStats.movementSpeed, TomeStatType.MovementSpeed);
        JumpHeightMultiplier = GetModifiedValue(baseStats.jumpHeightMultiplier, TomeStatType.JumpHeightMultiplier);
        ExtraJumps = Mathf.RoundToInt(GetModifiedValue(baseStats.extraJumps, TomeStatType.ExtraJumps));

        MaxHp = GetModifiedValue(baseStats.maxHp, TomeStatType.MaxHp);
        HpRegen = GetModifiedValue(baseStats.hpRegen, TomeStatType.HpRegen);
        Shield = GetModifiedValue(baseStats.shield, TomeStatType.Shield);
        Armor = GetModifiedValue(baseStats.armor, TomeStatType.Armor);

        Damage = GetModifiedValue(baseStats.damage, TomeStatType.Damage);
        AttackSpeedMultiplier = GetModifiedValue(baseStats.attackSpeedMultiplier, TomeStatType.AttackSpeedMultiplier);
        CritChance = GetModifiedValue(baseStats.critChance, TomeStatType.CritChance);
        CritDamageMultiplier = GetModifiedValue(baseStats.critDamageMultiplier, TomeStatType.CritDamageMultiplier);

        DodgeChance = GetModifiedValue(baseStats.dodgeChance, TomeStatType.DodgeChance);
        Lifesteal = GetModifiedValue(baseStats.lifesteal, TomeStatType.Lifesteal);
        Thorns = GetModifiedValue(baseStats.thorns, TomeStatType.Thorns);

        ProjectileSizeMultiplier = GetModifiedValue(baseStats.projectileSizeMultiplier, TomeStatType.ProjectileSizeMultiplier);
        ProjectileSpeedMultiplier = GetModifiedValue(baseStats.projectileSpeedMultiplier, TomeStatType.ProjectileSpeedMultiplier);
        KnockbackMultiplier = GetModifiedValue(baseStats.knockbackMultiplier, TomeStatType.KnockbackMultiplier);

        DurationMultiplier = GetModifiedValue(baseStats.durationMultiplier, TomeStatType.DurationMultiplier);
        LuckMultiplier = GetModifiedValue(baseStats.luckMultiplier, TomeStatType.LuckMultiplier);

        PickupRangeMultiplier = GetModifiedValue(baseStats.pickupRangeMultiplier, TomeStatType.PickupRangeMultiplier);
        XpGainMultiplier = GetModifiedValue(baseStats.xpGainMultiplier, TomeStatType.XpGainMultiplier);
        CurrencyGainMultiplier = GetModifiedValue(baseStats.currencyGainMultiplier, TomeStatType.CurrencyGainMultiplier);
    }

    private float GetModifiedValue(float baseValue, TomeStatType statType)
    {
        float flatBonus = 0f;
        float percentBonus = 0f;

        int playerLevel = LevelManager.Instance != null ? LevelManager.Instance.CurrentLevel : 1;

        foreach (var tome in activeTomes)
        {
            if (tome.statToModify != statType) continue;

            int effectiveLevel = playerLevel;
            if (tome.isLimit)
            {
                effectiveLevel = Mathf.Min(effectiveLevel, tome.maxLevel);
            }

            float tomeValue = tome.baseValue + (tome.scaleByLevel * (effectiveLevel - 1));

            if (tome.scalingType == TomeScalingType.Value)
            {
                flatBonus += tomeValue;
            }
            else if (tome.scalingType == TomeScalingType.Percentage)
            {
                percentBonus += tomeValue;
            }
        }

        return (baseValue + flatBonus) * (1f + percentBonus);
    }
}