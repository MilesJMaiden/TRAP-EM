using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public Transform[] patrolPoints; // Array of patrol points for the flying enemy
    public float flightSpeed = 5f; // Speed of the flying enemy
    public float rotationSpeed = 2f; // Speed at which the enemy rotates toward the next target
    public float closeEnoughDistance = 0.5f; // Distance considered close enough to a patrol point to move to the next
    public float chaseDistance = 20f; // Distance within which the enemy will start chasing the player
    public float obstacleDetectionRange = 5f; // Range for detecting obstacles in the air
    public float smoothAcceleration = 1f; // Smooth acceleration for gradual speed increase

    private int currentPatrolIndex = 0; // The index of the current patrol point
    private bool isChasing = false; // Whether the enemy is chasing the player
    private bool isReturningToPatrol = false; // Whether the enemy is returning to patrol after chasing

    private Vector3 currentTarget; // Current target point for patrolling or chasing
    private Rigidbody rb; // Rigidbody for physics-based flight control

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[currentPatrolIndex].position; // Start at the first patrol point
        }
    }

    void Update()
    {
        // Check if the player is within the chase distance
        if (Vector3.Distance(transform.position, player.position) < chaseDistance)
        {
            isChasing = true;
            currentTarget = player.position;
        }
        else
        {
            isChasing = false;
        }

        // Move the enemy
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Handle obstacle avoidance
        AvoidObstacles();
    }

    private void ChasePlayer()
    {
        // Smoothly rotate toward the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Smoothly accelerate toward the player
        rb.velocity = Vector3.Lerp(rb.velocity, directionToPlayer * flightSpeed, Time.deltaTime * smoothAcceleration);
    }

    private void Patrol()
    {
        // Smoothly rotate toward the current patrol point
        Vector3 directionToTarget = (currentTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Smoothly move toward the patrol point
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * flightSpeed, Time.deltaTime * smoothAcceleration);

        // Check if the enemy is close enough to the current patrol point to move to the next
        if (Vector3.Distance(transform.position, currentTarget) < closeEnoughDistance)
        {
            SetNextPatrolPoint();
        }
    }

    private void SetNextPatrolPoint()
    {
        // Move to the next patrol point in the array (loop back to the first after reaching the last)
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        currentTarget = patrolPoints[currentPatrolIndex].position;
    }

    private void AvoidObstacles()
    {
        // Raycast to detect obstacles in front of the flying enemy
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleDetectionRange))
        {
            if (hit.collider != null)
            {
                Debug.Log("Obstacle detected, avoiding.");
                // If obstacle detected, move in a direction away from the obstacle
                Vector3 avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                rb.velocity = avoidanceDirection * flightSpeed;
            }
        }
    }

    // Optional: You can visualize the patrol points and obstacle detection range for debugging
    private void OnDrawGizmos()
    {
        // Draw patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (var point in patrolPoints)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }

        // Draw obstacle detection range
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * obstacleDetectionRange);
    }
}
