using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public enum ItemType { Currency, Exp }
    public ItemType itemType;

    public int minAmount = 1;
    public int maxAmount = 5;

    public float lifetime = 10f;
    public float hopHeight = 1.2f;
    public float scatterDistance = 1.5f;
    public float animationDuration = 0.5f;
    public float gravity = -20f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float animationTimer = 0f;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    void Start()
    {
        Destroy(gameObject, lifetime);

        startPosition = transform.position;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0.6f, scatterDistance);

        Vector3 scatterOffset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        targetPosition = startPosition + scatterOffset;

        if (Physics.Raycast(targetPosition + Vector3.up * 3f, Vector3.down, out RaycastHit hit, 15f))
        {
            targetPosition.y = hit.point.y;
        }
    }

    void Update()
    {
        if (animationTimer < animationDuration)
        {
            animationTimer += Time.deltaTime;
            float progress = animationTimer / animationDuration;

            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPos.y += Mathf.Sin(progress * Mathf.PI) * hopHeight;

            transform.position = currentPos;
        }
        else if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.15f))
            {
                transform.position = hit.point;
                isGrounded = true;
                verticalVelocity = 0f;
            }
        }
    }
}