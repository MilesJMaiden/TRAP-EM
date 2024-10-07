using UnityEngine;
using System.Collections;

public class PatrolState : NPCState
{
    private float waitTime; // How long the NPC waits at patrol points
    private float waitTimer; // Tracks time spent waiting
    private bool isWaiting; // Whether the NPC is currently waiting at a patrol point
    private bool isRandomizingPath = true; // Randomize patrol order

    public PatrolState(NPCBase npcBase) : base(npcBase) { }

    public override void EnterState()
    {
        npc.agent.speed = npc.patrolSpeed;
        npc.isAlert = false;
        isWaiting = false;
        waitTime = Random.Range(2f, 5f); // Random wait time between 2 and 5 seconds
        MoveToNextPatrolPoint(); // Start patrol by moving to the first point
    }

    public override void Execute()
    {
        // If NPC is not alert, continue patrolling
        if (!npc.isAlert)
        {
            if (isWaiting)
            {
                // If waiting, track time and check if NPC should move to the next point
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTime)
                {
                    isWaiting = false;
                    MoveToNextPatrolPoint(); // Move to the next patrol point after waiting
                }
            }
            else if (!npc.agent.pathPending && npc.agent.remainingDistance <= npc.agent.stoppingDistance)
            {
                // NPC reached the patrol point, so start waiting and looking around
                StartWaiting();
            }
        }
        else
        {
            npc.TransitionToFlee(); // Switch to FleeState when alerted
        }
    }

    private void MoveToNextPatrolPoint()
    {
        // Ensure there are patrol points set
        if (npc.patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points assigned.");
            return;
        }

        // Randomize patrol order if enabled
        if (isRandomizingPath)
        {
            npc.currentPatrolPoint = Random.Range(0, npc.patrolPoints.Count);
        }
        else
        {
            // Move to the next patrol point in the list (loop back to the first after reaching the last)
            npc.currentPatrolPoint = (npc.currentPatrolPoint + 1) % npc.patrolPoints.Count;
        }

        // Set the NPC's destination to the next patrol point
        npc.agent.destination = npc.patrolPoints[npc.currentPatrolPoint].position;
        Debug.Log("Moving to patrol point: " + npc.patrolPoints[npc.currentPatrolPoint].name);
    }

    private void StartWaiting()
    {
        // Start waiting at the patrol point
        isWaiting = true;
        waitTimer = 0f;
        waitTime = Random.Range(2f, 5f); // Random wait time between 2 and 5 seconds

        // Simulate looking around while waiting
        npc.StartCoroutine(LookAround());
    }

    private IEnumerator LookAround()
    {
        float lookDuration = Random.Range(1f, 3f); // How long to look around
        float elapsed = 0f;

        // Rotate in a random direction while waiting
        while (elapsed < lookDuration && isWaiting)
        {
            float rotationSpeed = 30f; // Rotation speed while looking around
            npc.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop looking around after a random period of time
    }
}
