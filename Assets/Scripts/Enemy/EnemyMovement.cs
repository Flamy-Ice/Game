using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public float gravity = -20f;
    public float highJumpForce = 12.0f;
    public float wallCheckDistance = 1.0f;
    public float knockbackDecay = 8f;

    public float separationRadius = 1.5f;
    public float separationForce = 4.0f;

    public float climbSpeed = 5.0f;
    public float climbExitJumpForce = 9.0f;
    public float wallLostBuffer = 0.25f;

    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private LayerMask enemyLayer;

    private CharacterController controller;
    private Transform playerTransform;
    private EnemyStats enemyStats;
    private float verticalVelocity;

    private bool wasClimbing = false;
    private bool isJumping = false;
    private float wallLostTimer = 0f;
    private Vector3 knockbackVelocity = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        enemyStats = GetComponent<EnemyStats>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        knockbackVelocity = Vector3.MoveTowards(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);

        if (playerTransform == null || enemyStats == null) return;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();

            Vector3 targetVelocity = Vector3.zero;

            if (controller.isGrounded && verticalVelocity <= 0)
            {
                isJumping = false;
                wasClimbing = false;
                wallLostTimer = 0f;
            }

            bool isWallAhead = Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.3f, direction, out _, wallCheckDistance, obstacleLayers);

            if (isWallAhead && !isJumping)
            {
                wallLostTimer = 0f;
                verticalVelocity = climbSpeed;
                targetVelocity = (direction * (enemyStats.WalkSpeed * 0.3f)) + (Vector3.up * verticalVelocity);
                wasClimbing = true;
            }
            else if (wasClimbing && !isJumping)
            {
                wallLostTimer += Time.deltaTime;

                if (wallLostTimer < wallLostBuffer)
                {
                    verticalVelocity = climbSpeed;
                    targetVelocity = (direction * (enemyStats.WalkSpeed * 0.3f)) + (Vector3.up * verticalVelocity);
                }
                else
                {
                    verticalVelocity = climbExitJumpForce;
                    isJumping = true;
                    wasClimbing = false;
                    wallLostTimer = 0f;
                    targetVelocity = (direction * enemyStats.WalkSpeed) + (Vector3.up * verticalVelocity);
                }
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

                targetVelocity = direction * enemyStats.WalkSpeed;
                targetVelocity.y = verticalVelocity;
            }

            Vector3 separation = Vector3.zero;
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, separationRadius, enemyLayer);
            int avoidCount = 0;

            foreach (var enemy in nearbyEnemies)
            {
                if (enemy.gameObject != gameObject)
                {
                    Vector3 pushDir = transform.position - enemy.transform.position;
                    pushDir.y = 0;
                    float distance = pushDir.magnitude;

                    if (distance < separationRadius && distance > 0.001f)
                    {
                        separation += pushDir.normalized / distance;
                        avoidCount++;
                    }
                }
            }

            if (avoidCount > 0)
            {
                separation /= avoidCount;
            }

            Vector3 finalMovement = targetVelocity + knockbackVelocity + (separation * separationForce);
            controller.Move(finalMovement * Time.deltaTime);

            Vector3 lookDirection = -direction;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            Quaternion nextRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, nextRotation.eulerAngles.y, 0f);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.y = 0;
        direction.Normalize();
        knockbackVelocity = direction * force;
    }
}