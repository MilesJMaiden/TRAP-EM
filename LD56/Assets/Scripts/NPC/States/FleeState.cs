using UnityEngine;
using UnityEngine.AI;

public class FleeState : NPCState
{
    private Transform playerTransform;

    // Track how long the NPC has been safely away from the player
    private float timeOutsideFleeDistance = 0f;

    // Tracks time for weaving movement
    private float timeElapsed = 0f;

    // Store the last flee direction for reference
    private Vector3 lastFleeDirection;

    public FleeState(NPCBase npcBase, Transform player) : base(npcBase)
    {
        playerTransform = player;
    }

    public override void EnterState()
    {
        npc.agent.speed = Mathf.Lerp(npc.patrolSpeed, npc.alertSpeed, Time.deltaTime * 5);
        npc.isAlert = true;
        timeElapsed = 0f; // Reset movement timer
        lastFleeDirection = npc.transform.position - playerTransform.position; // Initialize flee direction
    }

    public override void Execute()
    {
        FleeFromPlayer();

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

    // Serpentine Movement: Smooth, sinusoidal weaving left and right
    private Vector3 ApplySerpentineMovement(Vector3 fleeDirection)
    {
        Vector3 perpendicular = Vector3.Cross(Vector3.up, fleeDirection).normalized;
        timeElapsed += Time.deltaTime;
        float oscillation = Mathf.Sin(timeElapsed * npc.weaveFrequency) * npc.weaveAmplitude;
        return (fleeDirection + perpendicular * oscillation).normalized;
    }

    // Zigzag Movement: More abrupt, linear left and right changes
    private Vector3 ApplyZigzagMovement(Vector3 fleeDirection)
    {
        Vector3 perpendicular = Vector3.Cross(Vector3.up, fleeDirection).normalized;
        timeElapsed += Time.deltaTime;
        float zigzag = (Mathf.Floor(timeElapsed * npc.weaveFrequency) % 2 == 0) ? npc.weaveAmplitude : -npc.weaveAmplitude;
        return (fleeDirection + perpendicular * zigzag).normalized;
    }

    // Random Jitter Movement: Adds small, random deviations to the direction
    private Vector3 ApplyRandomJitter(Vector3 fleeDirection)
    {
        float jitterX = Random.Range(-npc.jitterIntensity, npc.jitterIntensity);
        float jitterZ = Random.Range(-npc.jitterIntensity, npc.jitterIntensity);
        Vector3 jitter = new Vector3(jitterX, 0, jitterZ);
        return (fleeDirection + jitter).normalized;
    }

    // Circular Movement: Moves the NPC in a circular pattern while fleeing
    private Vector3 ApplyCircularMovement(Vector3 fleeDirection)
    {
        timeElapsed += Time.deltaTime;
        float angle = timeElapsed * npc.weaveFrequency;
        Vector3 circularOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * npc.circularMovementRadius;
        return (fleeDirection + circularOffset).normalized;
    }

    private void ReturnToNearestPatrolPoint()
    {
        // Find the nearest patrol point to return to
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
