using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private CharacterStatsData baseStats;

    public float MovementSpeed { get; private set; }
    public float JumpHeightMultiplier { get; private set; }
    public int ExtraJumps { get; private set; }

    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
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

    public void ApplyStats(CharacterStatsData data)
    {
        if (data == null) return;

        baseStats = data;

        MovementSpeed = data.movementSpeed;
        JumpHeightMultiplier = data.jumpHeightMultiplier;
        ExtraJumps = data.extraJumps;

        MaxHp = data.maxHp;
        CurrentHp = data.maxHp;
        HpRegen = data.hpRegen;
        Shield = data.shield;
        Armor = data.armor;

        Damage = data.damage;
        AttackSpeedMultiplier = data.attackSpeedMultiplier;
        CritChance = data.critChance;
        CritDamageMultiplier = data.critDamageMultiplier;

        DodgeChance = data.dodgeChance;
        Lifesteal = data.lifesteal;
        Thorns = data.thorns;

        ProjectileSizeMultiplier = data.projectileSizeMultiplier;
        ProjectileSpeedMultiplier = data.projectileSpeedMultiplier;
        KnockbackMultiplier = data.knockbackMultiplier;

        DurationMultiplier = data.durationMultiplier;
        LuckMultiplier = data.luckMultiplier;

        PickupRangeMultiplier = data.pickupRangeMultiplier;
        XpGainMultiplier = data.xpGainMultiplier;
        CurrencyGainMultiplier = data.currencyGainMultiplier;
    }
}