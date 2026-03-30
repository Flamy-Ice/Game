using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ustawienia Ruchu")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Mechanika Dasha (Uniku)")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private CharacterController controller;
    private Camera mainCamera;

    private Vector2 inputVector = Vector2.zero;
    private Vector3 playerVelocity;
    private Vector3 moveDirection;

    private bool isGrounded;
    private bool isDashing;
    private float lastDashTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main; // Pobieramy referencję do kamery
    }

    void Update()
    {
        if (isDashing) return; // Blokujemy zwykły ruch podczas dasha

        ApplyGravity();
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // Ruch relatywny do orientacji kamery (ważne w 3D!)
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * inputVector.y + right * inputVector.x).normalized;

        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // --- OBSŁUGA INPUTU (Z Input System) ---

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isDashing)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // Wykonaj dash tylko jeśli przycisk wciśnięty, nie jesteśmy w trakcie dasha i minął cooldown
        if (context.performed && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        // Jeśli postać stoi w miejscu, dashuj do przodu, w innym wypadku w stronę ruchu
        Vector3 dashDir = moveDirection == Vector3.zero ? transform.forward : moveDirection;

        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            // Przesuwamy postać o ułamek dystansu dasha
            controller.Move(dashDir * (dashDistance / dashTime) * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }
}