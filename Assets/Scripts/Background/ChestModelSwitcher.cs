using UnityEngine;
using System.Collections.Generic;

public class ChestModelSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> models;

    public void SwitchModel(int mapIndex)
    {
        if (models == null || models.Count == 0) return;

        int modelIndex = mapIndex % models.Count;

        for (int i = 0; i < models.Count; i++)
        {
            if (models[i] != null)
            {
                models[i].SetActive(i == modelIndex);
            }
        }
    }
}