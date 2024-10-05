using UnityEngine;
using UnityEngine.AI;

public class FleeState : NPCState
{
    private Transform playerTransform;

    // Designer-friendly parameters for serpentine movement
    public float weaveFrequency = 2f;   // How fast the NPC moves left and right
    public float weaveAmplitude = 2f;   // How far the NPC moves left and right
    public float fleeDistanceThreshold = 20f; // Distance NPC tries to keep away from the player
    public float fleeCheckDuration = 5f; // Time NPC needs to be safe before returning to patrol
    private float timeOutsideFleeDistance = 0f; // Track how long NPC is safely away from player

    private float timeElapsed = 0f; // Tracks time for weaving movement

    public FleeState(NPCBase npcBase, Transform player) : base(npcBase)
    {
        playerTransform = player;
    }

    public override void EnterState()
    {
        // Lerp to alert speed over time
        npc.agent.speed = Mathf.Lerp(npc.patrolSpeed, npc.alertSpeed, Time.deltaTime * 5);
        npc.isAlert = true;
        timeElapsed = 0f; // Reset weaving timer
    }

    public override void Execute()
    {
        FleeFromPlayer();

        // Check if NPC has maintained distance for the required period
        if (Vector3.Distance(npc.transform.position, playerTransform.position) > fleeDistanceThreshold)
        {
            timeOutsideFleeDistance += Time.deltaTime;

            // If NPC is safe for a certain amount of time, return to patrol state
            if (timeOutsideFleeDistance > fleeCheckDuration)
            {
                ReturnToNearestPatrolPoint();
            }
        }
        else
        {
            // Reset the timer if NPC is too close
            timeOutsideFleeDistance = 0;
        }
    }

    private void FleeFromPlayer()
    {
        // Find direction away from the player
        Vector3 fleeDirection = (npc.transform.position - playerTransform.position).normalized;

        // Add serpentine (weaving) effect to the flee direction
        Vector3 serpentineFleeDirection = AddSerpentineMovement(fleeDirection);

        // Set flee destination a certain distance in the adjusted flee direction
        Vector3 fleeDestination = npc.transform.position + serpentineFleeDirection * 10f; // Arbitrary flee distance multiplier

        npc.agent.destination = fleeDestination;
    }

    // Adds serpentine movement to the flee direction
    private Vector3 AddSerpentineMovement(Vector3 fleeDirection)
    {
        // Calculate the perpendicular direction to the flee direction (for left-right weaving)
        Vector3 perpendicular = Vector3.Cross(Vector3.up, fleeDirection).normalized;

        // Add a sinusoidal oscillation to the perpendicular direction
        timeElapsed += Time.deltaTime;
        float oscillation = Mathf.Sin(timeElapsed * weaveFrequency) * weaveAmplitude;

        // Combine the flee direction with the oscillation for serpentine motion
        Vector3 serpentineDirection = (fleeDirection + perpendicular * oscillation).normalized;
        return serpentineDirection;
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
