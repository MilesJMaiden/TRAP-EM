using UnityEngine;

public class FleeState : NPCState
{
    private Transform playerTransform;
    private float timeOutsideFleeDistance = 0f; // Track how long NPC is safely away from player
    private float timeElapsed = 0f; // Tracks time for weaving movement
    private Vector3 lastFleeDirection;

    // Timer to control how frequently objects are instantiated
    private float spawnCooldown = 1f; // Time between spawns
    private float spawnTimer = 0f; // Tracks time since last spawn

    // NEW: Variables for raycasting and obstacle detection
    private float obstacleDetectionDistance = 3f; // Distance to check for obstacles in front
    private float stuckCheckTimer = 0f; // Timer for checking if AI is stuck
    private float stuckThreshold = 2f; // Time threshold to determine if AI is stuck
    private Vector3 lastPosition; // Last recorded position to check if AI is stuck

    public FleeState(NPCBase npcBase, Transform player) : base(npcBase)
    {
        playerTransform = player;
    }

    public override void EnterState()
    {
        npc.agent.speed = Mathf.Lerp(npc.patrolSpeed, npc.alertSpeed, Time.deltaTime * 5);
        npc.isAlert = true;
        timeElapsed = 0f; // Reset movement timer
        timeOutsideFleeDistance = 0f; // Reset safe time tracking
        spawnTimer = 0f; // Reset spawn timer
        lastFleeDirection = npc.transform.position - playerTransform.position; // Initialize flee direction
        lastPosition = npc.transform.position; // Initialize last known position
    }

    public override void Execute()
    {
        FleeFromPlayer();

        // Control object instantiation while fleeing
        HandleFleeObjectSpawn();

        // Check if AI is stuck
        HandleStuckDetection();

        if (Vector3.Distance(npc.transform.position, playerTransform.position) > npc.fleeDistanceThreshold)
        {
            timeOutsideFleeDistance += Time.deltaTime;

            if (timeOutsideFleeDistance > npc.fleeCheckDuration)
            {
                ReturnToNearestPatrolPoint();
            }
        }
        else
        {
            timeOutsideFleeDistance = 0;
        }
    }

    private void FleeFromPlayer()
    {
        // Calculate base flee direction (away from the player)
        Vector3 fleeDirection = (npc.transform.position - playerTransform.position).normalized;

        // Check for obstacles and adjust the flee direction
        fleeDirection = AvoidObstacles(fleeDirection);

        // Apply movement variation based on enabled boolean
        if (npc.useSerpentineMovement)
        {
            fleeDirection = ApplySerpentineMovement(fleeDirection);
        }
        else if (npc.useZigzagMovement)
        {
            fleeDirection = ApplyZigzagMovement(fleeDirection);
        }
        else if (npc.useRandomJitterMovement)
        {
            fleeDirection = ApplyRandomJitter(fleeDirection);
        }
        else if (npc.useCircularMovement)
        {
            fleeDirection = ApplyCircularMovement(fleeDirection);
        }

        // Set the NPC destination
        Vector3 fleeDestination = npc.transform.position + fleeDirection * 10f;
        npc.agent.destination = fleeDestination;
    }

    private Vector3 ApplySerpentineMovement(Vector3 fleeDirection)
    {
        Vector3 perpendicular = Vector3.Cross(Vector3.up, fleeDirection).normalized;
        timeElapsed += Time.deltaTime;
        float oscillation = Mathf.Sin(timeElapsed * npc.weaveFrequency) * npc.weaveAmplitude;
        return (fleeDirection + perpendicular * oscillation).normalized;
    }

    private Vector3 ApplyZigzagMovement(Vector3 fleeDirection)
    {
        Vector3 perpendicular = Vector3.Cross(Vector3.up, fleeDirection).normalized;
        timeElapsed += Time.deltaTime;
        float zigzag = (Mathf.Floor(timeElapsed * npc.weaveFrequency) % 2 == 0) ? npc.weaveAmplitude : -npc.weaveAmplitude;
        return (fleeDirection + perpendicular * zigzag).normalized;
    }

    private Vector3 ApplyRandomJitter(Vector3 fleeDirection)
    {
        float jitterX = Random.Range(-npc.jitterIntensity, npc.jitterIntensity);
        float jitterZ = Random.Range(-npc.jitterIntensity, npc.jitterIntensity);
        Vector3 jitter = new Vector3(jitterX, 0, jitterZ);
        return (fleeDirection + jitter).normalized;
    }

    private Vector3 ApplyCircularMovement(Vector3 fleeDirection)
    {
        timeElapsed += Time.deltaTime;
        float angle = timeElapsed * npc.weaveFrequency;
        Vector3 circularOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * npc.circularMovementRadius;
        return (fleeDirection + circularOffset).normalized;
    }

    // Method to detect obstacles and adjust flee direction
    private Vector3 AvoidObstacles(Vector3 fleeDirection)
    {
        RaycastHit hit;
        Vector3 origin = npc.transform.position + Vector3.up; // Cast slightly above the ground

        // Raycast in the flee direction to check for obstacles
        if (Physics.Raycast(origin, fleeDirection, out hit, obstacleDetectionDistance))
        {
            if (hit.collider != null && hit.collider.gameObject != npc.gameObject)
            {
                Debug.Log("Obstacle detected, adjusting flee direction.");

                // Adjust the flee direction by adding a perpendicular vector to avoid the obstacle
                Vector3 avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

                // Combine the flee direction with the avoidance direction
                return (fleeDirection + avoidanceDirection).normalized;
            }
        }

        return fleeDirection; // Return the original flee direction if no obstacle detected
    }

    // Method to handle stuck detection
    private void HandleStuckDetection()
    {
        if (Vector3.Distance(npc.transform.position, lastPosition) < 0.1f)
        {
            stuckCheckTimer += Time.deltaTime;

            if (stuckCheckTimer > stuckThreshold)
            {
                Debug.Log("NPC is stuck, recalculating path.");

                // Reset the path or move in a random direction to get unstuck
                FleeInRandomDirection();
                stuckCheckTimer = 0f;
            }
        }
        else
        {
            stuckCheckTimer = 0f;
        }

        lastPosition = npc.transform.position;
    }

    private void FleeInRandomDirection()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 fleeDestination = npc.transform.position + randomDirection * 10f;
        npc.agent.destination = fleeDestination;
        Debug.Log("Fleeing in a random direction.");
    }

    private void HandleFleeObjectSpawn()
    {
        if (npc.fleeObjectPrefab == null)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnCooldown)
        {
            Vector3 spawnPosition = npc.transform.position - npc.transform.forward * 1.5f;
            GameObject instantiatedObject = Object.Instantiate(npc.fleeObjectPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Instantiated object behind NPC: " + instantiatedObject.name);

            spawnTimer = 0f;
        }
    }

    private void ReturnToNearestPatrolPoint()
    {
        Transform nearestPatrolPoint = null;
        float minDistance = Mathf.Infinity;

        foreach (var patrolPoint in npc.patrolPoints)
        {
            float distance = Vector3.Distance(npc.transform.position, patrolPoint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPatrolPoint = patrolPoint;
            }
        }

        if (nearestPatrolPoint != null)
        {
            npc.agent.destination = nearestPatrolPoint.position;
            npc.SetState(new PatrolState(npc));
            Debug.Log("Returning to patrol state.");
        }
    }
}
