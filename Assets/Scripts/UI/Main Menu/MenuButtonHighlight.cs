using UnityEngine;
using UnityEngine.EventSystems;

// This script moves and toggles "highlight" UI elements when the mouse hovers over menu buttons
public class MenuButtonHighlight : MonoBehaviour, IPointerEnterHandler
{
    public enum Side { Left, Right }

    [Header("Highlight References")]
    public RectTransform highlightLeft;  // The UI element that appears on the left side
    public RectTransform highlightRight; // The UI element that appears on the right side

    [Header("Button Settings")]
    public Side sideForThisButton;       // Which side should show the highlight for this specific button?
    public float moveSpeed = 5f;        // How fast the highlight slides to the new position

    // These are static so all buttons share the same target and initialization state
    private static float targetY;
    private static bool isInitialized = false;

    // Runs when the game starts
    void Start()
    {
        // Only run this setup once for all buttons
        if (!isInitialized)
        {
            // Set the initial target position based on where the highlights are currently placed
            if (highlightLeft != null) targetY = highlightLeft.position.y;
            else if (highlightRight != null) targetY = highlightRight.position.y;

            // Hide both highlights at the very beginning
            if (highlightLeft != null) highlightLeft.gameObject.SetActive(false);
            if (highlightRight != null) highlightRight.gameObject.SetActive(false);

            isInitialized = true;
        }
    }

    // Runs every frame to handle smooth movement
    void Update()
    {
        // Smoothly slide the Left highlight toward the target Y position
        if (highlightLeft != null)
        {
            float newY = Mathf.Lerp(highlightLeft.position.y, targetY, Time.deltaTime * moveSpeed);
            highlightLeft.position = new Vector3(highlightLeft.position.x, newY, highlightLeft.position.z);
        }

        // Smoothly slide the Right highlight toward the target Y position
        if (highlightRight != null)
        {
            float newY = Mathf.Lerp(highlightRight.position.y, targetY, Time.deltaTime * moveSpeed);
            highlightRight.position = new Vector3(highlightRight.position.x, newY, highlightRight.position.z);
        }
    }

    // This is automatically called by Unity when the mouse enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Tell the highlights to move to this button's vertical position
        targetY = transform.position.y;

        // Switch which highlight is visible based on this button's settings
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