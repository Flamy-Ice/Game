using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = 1.5f;

    private EnemyStats stats;
    private Transform player;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (playerGameObject != null)
        {
            player = playerGameObject.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }

        if (distance > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, stats.CurrentMoveSpeed * Time.deltaTime);
        }
    }
}