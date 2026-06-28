using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public string description;
    public Sprite icon;
    public GameObject weaponPrefab;
    public int maxLevel = 10;
}