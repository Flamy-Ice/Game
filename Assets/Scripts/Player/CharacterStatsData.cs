using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Characters/Character Stats")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Movement Stats")]
    public float moveSpeed = 10f;
    public float jumpHeightMultiplier = 1f;
    public int extraJumps = 1;
}