using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 3.0f;
    public float rotationSpeed = 5.0f;
    public float gravity = -20f;
    public float highJumpForce = 12.0f;
    public float wallCheckDistance = 0.8f;

    private CharacterController controller;
    private Transform playerTransform;
    private float verticalVelocity;

    private bool wasClimbing = false;
    private bool isJumping = false;
    private float lastYPosition;
    private float stuckTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        lastYPosition = transform.position.y;
    }

    void Update()
    {
        if (playerTransform == null) return;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();

            Vector3 targetVelocity = Vector3.zero;

            if (controller.isGrounded && verticalVelocity <= 0)
            {
                isJumping = false;
            }

            bool isWallAhead = Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, wallCheckDistance);

            if (isWallAhead && !isJumping)
            {
                if (Mathf.Abs(transform.position.y - lastYPosition) < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                }
                else
                {
                    stuckTimer = 0f;
                }
                lastYPosition = transform.position.y;

                if (stuckTimer > 0.3f)
                {
                    verticalVelocity = highJumpForce;
                    isJumping = true;
                    wasClimbing = false;
                    stuckTimer = 0f;
                    targetVelocity = direction * speed + Vector3.up * verticalVelocity;
                }
                else
                {
                    verticalVelocity = speed * 1.5f;
                    targetVelocity = direction * 0.2f + Vector3.up * verticalVelocity;
                    wasClimbing = true;
                }
            }
            else
            {
                stuckTimer = 0f;

                if (wasClimbing)
                {
                    verticalVelocity = highJumpForce;
                    isJumping = true;
                    wasClimbing = false;
                }
                else
                {
                    if (controller.isGrounded)
                    {
                        verticalVelocity = -2f;
                    }
                    else
                    {
                        verticalVelocity += gravity * Time.deltaTime;
                    }
                }

                targetVelocity = direction * speed;
                targetVelocity.y = verticalVelocity;
            }

            controller.Move(targetVelocity * Time.deltaTime);

            Vector3 lookDirection = -direction;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            Quaternion nextRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, nextRotation.eulerAngles.y, 0f);
        }
    }
}