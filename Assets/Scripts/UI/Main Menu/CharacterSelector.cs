using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] characterModels;

    void Start()
    {
        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterModels.Length) return;

        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null)
            {
                characterModels[i].SetActive(i == index);
            }
        }
    }

    public void HideAllCharacters()
    {
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null) characterModels[i].SetActive(false);
        }
    }
}