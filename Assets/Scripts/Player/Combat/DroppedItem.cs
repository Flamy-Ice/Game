using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public float lifetime = 10f;
    public float hopHeight = 1.2f;
    public float scatterDistance = 1.5f;
    public float animationDuration = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float animationTimer = 0f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        startPosition = transform.position;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0.6f, scatterDistance);

        Vector3 scatterOffset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        targetPosition = startPosition + scatterOffset;
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
    }
}