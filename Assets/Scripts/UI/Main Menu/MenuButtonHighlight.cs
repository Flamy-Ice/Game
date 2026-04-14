using UnityEngine;
using UnityEngine.EventSystems; // Ważne dla obsługi myszki

public class MenuButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Tutaj wrzucimy naszą gwiazdkę w inspektorze
    public GameObject highlightElement;

    void Start()
    {
        // Na wszelki wypadek chowamy gwiazdkę na początku gry
        if (highlightElement != null)
            highlightElement.SetActive(false);
    }

    // Wywołuje się, gdy myszka najedzie na przycisk
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightElement != null)
            highlightElement.SetActive(true);
    }

    // Wywołuje się, gdy myszka zjedzie z przycisku
    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightElement != null)
            highlightElement.SetActive(false);
    }
}