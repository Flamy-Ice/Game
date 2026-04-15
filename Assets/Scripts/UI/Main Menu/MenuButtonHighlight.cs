using UnityEngine;
using UnityEngine.EventSystems;

// This script handles moving UI highlight bars to the currently selected button.
public class MenuButtonHighlight : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler
{
    public enum Side { Left, Right }

    [Header("Highlight References")]
    public RectTransform highlightLeft;  // The visual bar for buttons on the left
    public RectTransform highlightRight; // The visual bar for buttons on the right

    [Header("Button Settings")]
    public Side sideForThisButton;       // Should the highlight appear on the left or right for this button?
    public float moveSpeed = 12f;        // How fast the bar slides up and down

    // These are static so all buttons share the same highlight position and state
    private static float targetY;
    private static bool isInitialized = false;

    // Runs once when the button is first created
    void Start()
    {
        if (!isInitialized)
        {
            // Set the initial height based on whichever highlight is available
            if (highlightLeft != null) targetY = highlightLeft.position.y;
            else if (highlightRight != null) targetY = highlightRight.position.y;

            // Hide the bars until a button is actually selected
            if (highlightLeft != null) highlightLeft.gameObject.SetActive(false);
            if (highlightRight != null) highlightRight.gameObject.SetActive(false);

            isInitialized = true;
        }
    }

    // Runs every frame to handle the smooth sliding animation
    void Update()
    {
        // Smoothly move the Left highlight to the target height
        if (highlightLeft != null && highlightLeft.gameObject.activeInHierarchy)
        {
            float newY = Mathf.Lerp(highlightLeft.position.y, targetY, Time.deltaTime * moveSpeed);
            highlightLeft.position = new Vector3(highlightLeft.position.x, newY, highlightLeft.position.z);
        }

        // Smoothly move the Right highlight to the target height
        if (highlightRight != null && highlightRight.gameObject.activeInHierarchy)
        {
            float newY = Mathf.Lerp(highlightRight.position.y, targetY, Time.deltaTime * moveSpeed);
            highlightRight.position = new Vector3(highlightRight.position.x, newY, highlightRight.position.z);
        }
    }

    #region Input Events
    // When the mouse hovers over the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // When the button is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // When the button becomes the "Active" one (via mouse or keyboard/controller)
    public void OnSelect(BaseEventData eventData)
    {
        ActivateHighlight();
    }
    #endregion

    // Moves the shared highlight to this button's position and switches sides if needed
    private void ActivateHighlight()
    {
        // Tell the static bars to move to this button's Y position
        targetY = transform.position.y;

        // Toggle the correct highlight bar based on this button's settings
        if (sideForThisButton == Side.Left)
        {
            if (highlightLeft != null) highlightLeft.gameObject.SetActive(true);
            if (highlightRight != null) highlightRight.gameObject.SetActive(false);
        }
        else
        {
            if (highlightLeft != null) highlightLeft.gameObject.SetActive(false);
            if (highlightRight != null) highlightRight.gameObject.SetActive(true);
        }
    }
}