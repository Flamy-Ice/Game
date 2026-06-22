using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 3.0f;
    public float rotationSpeed = 5.0f;
    public float gravity = -20f;

    private CharacterController controller;
    private Transform playerTransform;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();

            Vector3 targetVelocity = direction * speed;

            if (controller.isGrounded)
            {
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            targetVelocity.y = verticalVelocity;

            controller.Move(targetVelocity * Time.deltaTime);

            Vector3 lookDirection = -direction;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            Quaternion nextRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, nextRotation.eulerAngles.y, 0f);
        }
    }
}