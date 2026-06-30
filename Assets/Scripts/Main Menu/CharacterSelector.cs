using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    [System.Serializable]
    public struct CharacterData
    {
        public string characterName;
        public GameObject model;
        public CharacterStatsData stats;
        public GameObject characterCanvas;
    }

    [SerializeField] private CharacterData[] characters;
    [SerializeField] private PlayerStatsUI statsUI;
    [SerializeField] private string gameSceneName = "GameScene";

    private int currentSelectedIndex = 0;

    void Start()
    {
        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Length) return;

        currentSelectedIndex = index;

        for (int i = 0; i < characters.Length; i++)
        {
            bool isActive = (i == index);

            if (characters[i].model != null) characters[i].model.SetActive(isActive);
            if (characters[i].characterCanvas != null) characters[i].characterCanvas.SetActive(isActive);

            if (isActive && statsUI != null && characters[i].stats != null)
            {
                statsUI.DisplayStats(characters[i].stats);
            }
        }
    }

    public void ConfirmSelection()
    {
        if (characters[currentSelectedIndex].stats != null)
        {
            CharacterTransfer.SelectedStats = characters[currentSelectedIndex].stats;
            CharacterTransfer.SelectedIndex = currentSelectedIndex;

            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void HideAllCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].model != null) characters[i].model.SetActive(false);
            if (characters[i].characterCanvas != null) characters[i].characterCanvas.SetActive(false);
        }
    }
}