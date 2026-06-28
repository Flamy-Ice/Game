using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<WeaponData> startingWeapons;

    private Dictionary<WeaponData, int> activeWeapons = new Dictionary<WeaponData, int>();
    private Dictionary<WeaponData, GameObject> instantiatedWeapons = new Dictionary<WeaponData, GameObject>();

    private void Start()
    {
        foreach (var weapon in startingWeapons)
        {
            if (weapon != null)
            {
                AddOrUpgradeWeapon(weapon);
            }
        }
    }

    public bool HasWeapon(WeaponData weapon)
    {
        return activeWeapons.ContainsKey(weapon);
    }

    public int GetWeaponLevel(WeaponData weapon)
    {
        if (activeWeapons.TryGetValue(weapon, out int level))
        {
            return level;
        }
        return 0;
    }

    public void AddOrUpgradeWeapon(WeaponData weaponData)
    {
        if (activeWeapons.ContainsKey(weaponData))
        {
            if (activeWeapons[weaponData] < weaponData.maxLevel)
            {
                activeWeapons[weaponData]++;
                UpdateWeaponInstance(weaponData);
            }
        }
        else
        {
            activeWeapons.Add(weaponData, 1);
            GameObject weaponInstance = Instantiate(weaponData.weaponPrefab, transform);
            instantiatedWeapons.Add(weaponData, weaponInstance);
            UpdateWeaponInstance(weaponData);
        }
    }

    private void UpdateWeaponInstance(WeaponData weaponData)
    {
        if (instantiatedWeapons.TryGetValue(weaponData, out GameObject instance))
        {
            IWeapon weaponScript = instance.GetComponent<IWeapon>();
            if (weaponScript != null)
            {
                weaponScript.SetLevel(activeWeapons[weaponData]);
            }
        }
    }
}