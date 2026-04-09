using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// This script controls how your player moves, jumps, and dashes.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Internal references to components
    private CharacterController _controller; // The "physical" body of the player
    private Transform _cameraTransform;      // Where the camera is looking
    private Vector2 _moveInput;             // Stores joystick or WASD input
    private Vector3 _moveDirection;         // The direction we are currently moving
    private Vector3 _velocity;              // Used for falling and jumping (vertical speed)

    // These settings appear in the Unity Inspector so you can tweak them
    [Header("Walk")]
    [SerializeField] private float _moveSpeed = 12f;         // How fast we run
    [SerializeField][Range(0f, 1f)] private float _walkModifier = 0.4f; // Slower speed when walking
    [SerializeField] private float _acceleration = 10f;     // How fast we reach top speed
    private bool _isWalking = false;

    [Header("Jump")]
    [SerializeField] private float _gravity = -35f;         // How hard the world pulls us down
    [SerializeField] private int _maxJumps = 2;             // How many times we can jump (Double Jump)
    [SerializeField] private float _jumpCooldown = 0.2f;    // Pause between jumps
    [SerializeField] private float _jumpHeight = 2.5f;      // How high the jump goes
    [SerializeField][Range(0f, 1f)] private float _airControl = 0.6f; // How much we can steer in mid-air
    [SerializeField][Range(0f, 1f)] private float _coyoteTime = 0.15f; // "Grace period" to jump after leaving a ledge
    [SerializeField][Range(0f, 1f)] private float _jumpBufferTime = 0.2f; // Remembers your jump tap if you hit it early

    private int _jumpsLeft;
    private float _coyoteCounter;
    private float _jumpBufferCounter;
    private float _lastJumpTime;

    [Header("Dash")]
    [SerializeField] private float _dashDistance = 12f;
    [SerializeField] private float _dashCooldown = 3f;
    [SerializeField] private float _dashTime = 0.25f;       // How long the dash lasts
    [Tooltip("How the speed changes during the dash (e.g., fast start, slow end)")]
    [SerializeField] private AnimationCurve _dashCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private bool _isDashing;
    private bool _canDash = true;
    public bool IsInvulnerable { get; private set; } // Can be used to make player immortal during dash

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 20f;    // How fast the player turns to face the move direction

    // This runs once when the game starts
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (Camera.main != null) _cameraTransform = Camera.main.transform;
        _jumpsLeft = _maxJumps;
    }

    // This runs every single frame (the "brain" of the player)
    private void Update()
    {
        HandleGroundCheck(); // Check if we are on the floor
        HandleRotation();    // Rotate player to face where they move
        HandleMovement();    // Move left/right/forward/back
        HandleGravity();     // Pull player down to the ground
    }

    #region Input Methods (These react to your button presses)
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>(); // Get WASD or Joystick values
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Prevent jump buffering if we are currently dashing
        if (_isDashing) return;

        if (context.started)
        {
            _jumpBufferCounter = _jumpBufferTime; // Mark that we want to jump
        }
    }

    public void OnWalkToggle(InputAction.CallbackContext context)
    {
        if (context.performed) _isWalking = true;
        else if (context.canceled) _isWalking = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && _canDash && !_isDashing)
        {
            StartCoroutine(PerformDash()); // Start the special Dash sequence
        }
    }
    #endregion

    private void HandleGroundCheck()
    {
        // We allow ground check during dash to ensure proper landing logic
        // If touching the ground, reset the jump counts
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Slight downward force to keep us stuck to the floor
            _jumpsLeft = _maxJumps;
            _coyoteCounter = _coyoteTime;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime; // Start the "falling off ledge" timer
        }

        _jumpBufferCounter -= Time.deltaTime; // Decrease the "early jump" timer

        // Decide if it's okay to jump now
        if (_jumpBufferCounter > 0 && (_coyoteCounter > 0 || _jumpsLeft > 0))
        {
            if (Time.time >= _lastJumpTime + _jumpCooldown)
            {
                Jump();
            }
        }
    }

    private void HandleMovement()
    {
        if (_isDashing) return; // Don't allow normal movement while dashing

        // Calculate direction based on where the camera is looking
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0; // Keep movement on the flat ground
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 targetDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;

        // Choose speed (Walking vs Running)
        float currentSpeed = _isWalking ? _moveSpeed * _walkModifier : _moveSpeed;

        // Make it harder to steer if we are in the air
        float controlModifier = _controller.isGrounded ? 1f : _airControl;

        // Smoothly speed up or slow down
        _moveDirection = Vector3.Lerp(_moveDirection, targetDirection * currentSpeed, _acceleration * controlModifier * Time.deltaTime);

        // Tell the controller to actually move the player
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (_moveDirection.sqrMagnitude < 0.01f || _isDashing) return;

        // Face the direction we are walking
        Vector3 lookDir = new Vector3(_moveDirection.x, 0, _moveDirection.z);
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            // Smoothly turn toward the target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        // Gravity is now applied even while dashing so the player can fall
        // Build up falling speed over time
        _velocity.y += _gravity * Time.deltaTime;
        // Apply that falling speed
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        // Math formula to calculate how much "push" is needed to reach a specific height
        _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        _jumpsLeft--;
        _jumpBufferCounter = 0; // Reset timers so we don't jump twice instantly
        _coyoteCounter = 0;
        _lastJumpTime = Time.time;
    }

    // A special "Coroutine" to handle the dash over a period of time
    private IEnumerator PerformDash()
    {
        _canDash = false;
        _isDashing = true;
        IsInvulnerable = true;
        _jumpBufferCounter = 0; // Clear any jumps that were queued right before the dash

        // Decide which way to dash (face forward, or follow movement keys)
        Vector3 dashDir = transform.forward;
        if (_moveInput.magnitude > 0.1f)
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;
            forward.y = 0; right.y = 0;
            dashDir = (forward * _moveInput.y + right * _moveInput.x).normalized;
        }

        float elapsed = 0f;
        float baseSpeed = _dashDistance / _dashTime;

        // The Dash Loop: runs every frame until the dash time is up
        while (elapsed < _dashTime)
        {
            float normalizedTime = elapsed / _dashTime;
            // Use the "Curve" to decide if we are zooming or slowing down
            float speedModifier = _dashCurve.Evaluate(normalizedTime);

            _controller.Move(dashDir * baseSpeed * speedModifier * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Keep a little bit of momentum after the dash ends
        _moveDirection = dashDir * (_moveSpeed * 0.5f);

        _isDashing = false;
        IsInvulnerable = false;

        // Wait for the cooldown before allowing another dash
        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }
}