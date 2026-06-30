using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject characterSelectionCanvas;
    [SerializeField] private GameObject creditsCanvas;

    public void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        characterSelectionCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void ShowCharacterSelection()
    {
        mainMenuCanvas.SetActive(false);
        characterSelectionCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuCanvas.SetActive(false);
        characterSelectionCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void HideAllMenus()
    {
        mainMenuCanvas.SetActive(false);
        characterSelectionCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }
}