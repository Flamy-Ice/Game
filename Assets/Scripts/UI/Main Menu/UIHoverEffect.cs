using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ustawienia Skalowania")]
    [SerializeField] private Vector3 hoveredScale = new Vector3(1.05f, 1.05f, 1.05f);
    [SerializeField] private float transitionSpeed = 12f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, transitionSpeed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoveredScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    void OnDisable()
    {
        transform.localScale = originalScale;
        targetScale = originalScale;
    }
}