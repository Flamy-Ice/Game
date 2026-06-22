using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] characterModels;

    void Start()
    {
        int index = CharacterTransfer.SelectedIndex;

        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null)
            {
                characterModels[i].SetActive(i == index);
            }
        }
    }
}