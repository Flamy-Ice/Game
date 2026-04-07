using UnityEngine;
using UnityEngine.AI;

// This script controls how an enemy follows the player using Unity's NavMesh system.
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _updateInterval = 0.2f; // How often to recalculate the path (saves CPU)

    // Internal references
    private NavMeshAgent _agent;      // The AI component that handles pathfinding
    private Transform _playerTransform; // The target the enemy is chasing
    private float _timer;             // Tracks time between path updates

    // This runs once when the game starts
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Optional: You can let the agent handle rotation or do it manually
        // _agent.updateRotation = true; 
    }

    // This runs before the first frame update
    private void Start()
    {
        // Find the player in the scene using their "Player" Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("EnemyMovement: No object with the 'Player' tag found!");
        }
    }

    // This runs every single frame
    private void Update()
    {
        if (_playerTransform == null) return;

        // Optimization: Don't calculate a new path every frame (expensive)
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            // Tell the AI to move toward the player's current position
            _agent.SetDestination(_playerTransform.position);
            _timer = _updateInterval; // Reset the cooldown timer
        }
    }

    // Call this to change speed on the fly (e.g., for slows or stuns)
    public void SetSpeed(float newSpeed)
    {
        _agent.speed = newSpeed;
    }
}