using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.5f;

    private EnemyStats stats;
    private Transform player;
    private PlayerHealth playerHealth;
    private float attackCooldownTimer;

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
            playerHealth = playerGameObject.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null) return;

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && attackCooldownTimer <= 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        playerHealth.TakeDamage(stats.CurrentDamage);
        attackCooldownTimer = stats.CurrentAttackSpeed;
    }
}