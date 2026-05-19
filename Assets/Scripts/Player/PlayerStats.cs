using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Character Configuration")]
    [SerializeField] private CharacterStatsData defaultStats;

    public float CurrentMoveSpeed { get; private set; }
    public float CurrentJumpHeightMultiplier { get; private set; }
    public int CurrentExtraJumps { get; private set; }

    private void Awake()
    {
        if (defaultStats != null)
        {
            InitializeStats();
        }
        else
        {
            Debug.LogError($"[PlayerStats] Missing 'Default Stats' asset on object: {gameObject.name}!");
        }
    }

    private void InitializeStats()
    {
        CurrentMoveSpeed = defaultStats.moveSpeed;
        CurrentJumpHeightMultiplier = defaultStats.jumpHeightMultiplier;
        CurrentExtraJumps = defaultStats.extraJumps;
    }
}