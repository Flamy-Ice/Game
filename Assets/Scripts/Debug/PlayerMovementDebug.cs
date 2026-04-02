using UnityEngine;
using System.Reflection;

// This script shows a "status window" on your screen to help you test the movement.
public class PlayerMovementDebug : MonoBehaviour
{
    private PlayerMovement _playerMovement; // Link to the main movement script
    private CharacterController _controller; // Link to the physical body

    [Header("Debugging Settings")]
    [SerializeField] private bool _showGui = true;    // Toggle the text box on screen
    [SerializeField] private bool _showGizmos = true; // Toggle the visual lines in the editor
    [SerializeField] private Color _gizmoColor = Color.cyan;

    // These track the Dash timer specifically for the display
    private float _dashTimer;
    private bool _lastCanDashState = true;

    private void Awake()
    {
        // Get the components when the game starts
        _playerMovement = GetComponent<PlayerMovement>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_playerMovement == null) return;

        // "Peeking" into the other script to see if we are allowed to dash
        bool canDash = GetPrivateField<bool>("_canDash");

        // If we just used the dash, start a countdown timer for the UI
        if (_lastCanDashState && !canDash)
        {
            float cooldown = GetPrivateField<float>("_dashCooldown");
            float duration = GetPrivateField<float>("_dashTime");
            _dashTimer = cooldown + duration;
        }

        // Count down the timer
        if (!canDash)
        {
            _dashTimer -= Time.deltaTime;
        }
        else
        {
            _dashTimer = 0;
        }

        _lastCanDashState = canDash;
    }

    // This method draws the text box you see while playing the game
    private void OnGUI()
    {
        if (!_showGui || _playerMovement == null) return;

        // Set up how the text looks
        GUIStyle style = new GUIStyle();
        style.fontSize = 12;
        style.richText = true;
        style.normal.textColor = Color.white;

        // Draw a background box in the top-left corner
        Rect boxRect = new Rect(10, 10, 210, 180);
        GUI.Box(boxRect, "Player Movement Debug");

        // Start putting text inside that box
        GUILayout.BeginArea(new Rect(boxRect.x + 10, boxRect.y + 25, boxRect.width - 20, boxRect.height - 30));

        // Get "Hidden" data from the PlayerMovement script
        float currentYVelocity = GetPrivateField<Vector3>("_velocity").y;
        Vector3 moveDir = GetPrivateField<Vector3>("_moveDirection");
        int jumps = GetPrivateField<int>("_jumpsLeft");
        bool isDashing = GetPrivateField<bool>("_isDashing");
        bool canDash = GetPrivateField<bool>("_canDash");
        float coyote = GetPrivateField<float>("_coyoteCounter");

        // Display the data as text
        GUILayout.Label($"Grounded: {ColorBool(_controller.isGrounded)}", style); // Are we on the floor?
        GUILayout.Label($"Speed: {new Vector3(moveDir.x, 0, moveDir.z).magnitude:F2} m/s", style); // Current speed
        GUILayout.Label($"Y Velocity: {currentYVelocity:F2}", style); // Falling/Jumping speed

        GUILayout.Space(5);

        GUILayout.Label($"Jumps Left: {jumps}", style); // How many mid-air jumps left
        GUILayout.Label($"Coyote Time: {Mathf.Max(0, coyote):F2}s", style); // Time left to jump after falling

        GUILayout.Space(5);

        GUILayout.Label($"Dashing: {ColorBool(isDashing)}", style);

        // Show "Dash Ready" - turns green if True, red with a timer if False
        string dashStatus = canDash
            ? "<color=#00FF00>True</color>"
            : $"<color=#FF0000>False</color> ({Mathf.Max(0, _dashTimer):F2}s)";
        GUILayout.Label($"Dash Ready: {dashStatus}", style);

        GUILayout.Label($"Invulnerable: {ColorBool(_playerMovement.IsInvulnerable)}", style);

        GUILayout.EndArea();
    }

    // Simple helper: converts true/false into Green/Red text
    private string ColorBool(bool value)
    {
        return value ? "<color=#00FF00>True</color>" : "<color=#FF0000>False</color>";
    }

    // This draws 3D lines and shapes that only developers can see in the Scene view
    private void OnDrawGizmos()
    {
        if (!_showGizmos || _playerMovement == null || _controller == null) return;

        // Draw a Cyan arrow showing which way the player is trying to move
        Vector3 moveDir = GetPrivateField<Vector3>("_moveDirection");
        Gizmos.color = _gizmoColor;
        DrawArrow(transform.position + Vector3.up, moveDir, "Move Dir");

        // Draw a Yellow arrow showing the current falling/jumping force
        Vector3 velocity = GetPrivateField<Vector3>("_velocity");
        Gizmos.color = Color.yellow;
        DrawArrow(transform.position + Vector3.up, velocity * 0.5f, "Velocity");

        // Draw a sphere at the player's feet (Green = Grounded, Red = In Air)
        Gizmos.color = _controller.isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    // THE "MAGIC" METHOD:
    // Normally, scripts can't see "private" variables in other scripts.
    // This uses "Reflection" to reach inside and grab that data anyway.
    private T GetPrivateField<T>(string fieldName)
    {
        FieldInfo field = typeof(PlayerMovement).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return field != null ? (T)field.GetValue(_playerMovement) : default;
    }

    // Helper to draw an arrow with a text label in the 3D world
    private void DrawArrow(Vector3 pos, Vector3 direction, string label)
    {
        if (direction.magnitude < 0.01f) return;
        Gizmos.DrawRay(pos, direction);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(pos + direction, label);
#endif
    }
}