using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset reference;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Walk")]
    [SerializeField][Range(0f, 1f)] private float walkSpeedMultiplier = 0.4f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 25f;

    [Header("Jump")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float baseJumpHeight = 2f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField][Range(0f, 1f)] private float airControl = 0.6f;
    [SerializeField][Range(0f, 1f)] private float coyoteTime = 0.15f;
    [SerializeField][Range(0f, 1f)] private float jumpBufferTime = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource jumpAudioSource;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction walkAction;

    private CharacterController controller;
    private PlayerStats stats;

    private Vector3 currentHorizontalVelocity;
    private float verticalVelocity;
    private bool isGrounded;

    private int remainingJumps;
    private bool hasJumped;
    private float jumpCooldownTimer;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();

        if (reference != null)
        {
            moveAction = reference.FindAction("Player/Move");
            jumpAction = reference.FindAction("Player/Jump");
            walkAction = reference.FindAction("Player/Walk");
        }
        else
        {
            Debug.LogError($"[PlayerMovement] Missing 'Reference' (Input Actions Asset) on object {gameObject.name}!");
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (walkAction != null) walkAction.Enable();

        if (jumpAction != null)
        {
            jumpAction.Enable();
            jumpAction.started += OnJumpPerformed;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (walkAction != null) walkAction.Disable();

        if (jumpAction != null)
        {
            jumpAction.Disable();
            jumpAction.started -= OnJumpPerformed;
        }
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        UpdateTimers();
        HandleJumpLogic();
        CalculateHorizontalMovement();

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 finalVelocity = currentHorizontalVelocity + new Vector3(0f, verticalVelocity, 0f);
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void UpdateTimers()
    {
        if (jumpCooldownTimer > 0) jumpCooldownTimer -= Time.deltaTime;
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        if (isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;
            coyoteTimeCounter = coyoteTime;
            remainingJumps = stats.ExtraJumps;
            hasJumped = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void HandleJumpLogic()
    {
        if (jumpBufferCounter > 0f && jumpCooldownTimer <= 0f)
        {
            bool canNormalJump = !hasJumped && (isGrounded || coyoteTimeCounter > 0f);

            if (canNormalJump)
            {
                ExecuteJump();
                hasJumped = true;
            }
            else if (remainingJumps > 0)
            {
                ExecuteJump();
                remainingJumps--;
            }
        }
    }

    private void ExecuteJump()
    {
        float effectiveJumpHeight = baseJumpHeight * stats.JumpHeightMultiplier;
        verticalVelocity = Mathf.Sqrt(effectiveJumpHeight * -2f * gravity);

        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }

        jumpBufferCounter = 0f;
        jumpCooldownTimer = jumpCooldown;
        coyoteTimeCounter = 0f;
    }

    private void CalculateHorizontalMovement()
    {
        Vector2 inputVector = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector3 targetMoveDirection = Vector3.zero;

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            targetMoveDirection = (camForward * inputVector.y) + (camRight * inputVector.x);
        }
        else
        {
            targetMoveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        }

        if (targetMoveDirection.magnitude > 1f)
        {
            targetMoveDirection.Normalize();
        }

        if (targetMoveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float targetSpeed = stats.MovementSpeed;

        if (walkAction != null && walkAction.IsPressed())
        {
            targetSpeed *= walkSpeedMultiplier;
        }

        Vector3 targetVelocity = targetMoveDirection * targetSpeed;

        bool isTryingToMove = targetVelocity.magnitude > 0f;
        float currentRate = isTryingToMove ? acceleration : deceleration;

        if (!isGrounded)
        {
            currentRate *= airControl;
        }

        currentHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetVelocity, currentRate * Time.deltaTime);
    }
}